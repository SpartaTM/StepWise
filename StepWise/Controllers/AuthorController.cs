using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StepWise.Data;
using StepWise.Models;
using StepWise.ViewModels;
using StepWise.Services;

namespace StepWise.Controllers;

public class AuthorController : Controller
{

    private readonly AppDbContext _db;
    private readonly ICurrentUser _currentUser;
    private string CurrentAuthorId => _currentUser.UserId;
    private const int StepSlots = 10;

    public AuthorController(AppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public IActionResult Index()
    {
        var items = _db.Instructions
            .AsNoTracking()
            .Where(i => i.AuthorId == CurrentAuthorId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new InstructionListItemViewModel
            {
                Id = i.Id,
                Title = i.Title,
                Category = i.Category,
                StepsCount = i.Steps.Count
            })
            .ToList();

        return View(items);
    }

    public IActionResult Create()
    {
        var model = new InstructionFormViewModel();

        PrepareForm(model);
        return View("Form", model); 
    }

    // Нажата кнопка «Сохранить»
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(InstructionFormViewModel model)
    {
        // Берём только заполненные шаги
        var filledSteps = model.Steps
            .Where(s => !string.IsNullOrWhiteSpace(s.Title))
            .ToList();

        if (filledSteps.Count == 0)
            ModelState.AddModelError(string.Empty, "Добавьте хотя бы один шаг");

        if (!ModelState.IsValid)
        {
            PrepareForm(model);
            return View("Form", model);   // показываем форму снова с ошибками
        }

        var instruction = new Instruction
        {
            Title = model.Title.Trim(),
            Description = model.Description.Trim(),
            Category = model.Category.Trim(),
            AuthorId = CurrentAuthorId,
            Steps = filledSteps
                .Select((s, index) => new Step
                {
                    Order = index + 1,   // нумеруем заново без пропусков
                    Title = s.Title!.Trim(),
                    Description = s.Description?.Trim() ?? string.Empty
                })
                .ToList()
        };

        _db.Instructions.Add(instruction);
        _db.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
    public IActionResult Edit(int id)
    {
        var instruction = _db.Instructions
            .AsNoTracking()
            .Include(i => i.Steps)
            .FirstOrDefault(i => i.Id == id && i.AuthorId == CurrentAuthorId);

        if (instruction == null)
            return NotFound();

        var model = new InstructionFormViewModel
        {
            Id = instruction.Id,
            Title = instruction.Title,
            Description = instruction.Description,
            Category = instruction.Category,
            Steps = instruction.Steps
                .OrderBy(s => s.Order)
                .Select(s => new StepFormItem
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description
                })
                .ToList()
        };

        PrepareForm(model);
        return View("Form", model);
    }

    // Нажата кнопка «Сохранить изменения»
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, InstructionFormViewModel model)
    {
        var instruction = _db.Instructions
            .Include(i => i.Steps)
            .FirstOrDefault(i => i.Id == id && i.AuthorId == CurrentAuthorId);

        if (instruction == null)
            return NotFound();

        var filledSteps = model.Steps
            .Where(s => !string.IsNullOrWhiteSpace(s.Title))
            .ToList();

        if (filledSteps.Count == 0)
            ModelState.AddModelError(string.Empty, "Добавьте хотя бы один шаг");

        if (!ModelState.IsValid)
        {
            PrepareForm(model);
            return View("Form", model);
        }

        instruction.Title = model.Title.Trim();
        instruction.Description = model.Description.Trim();
        instruction.Category = model.Category.Trim();

        var keepIds = filledSteps
            .Where(s => s.Id.HasValue)
            .Select(s => s.Id!.Value)
            .ToList();
        var toRemove = instruction.Steps.Where(s => !keepIds.Contains(s.Id)).ToList();
        _db.Steps.RemoveRange(toRemove);

        for (int index = 0; index < filledSteps.Count; index++)
        {
            var item = filledSteps[index];

            var step = item.Id.HasValue
                ? instruction.Steps.FirstOrDefault(s => s.Id == item.Id.Value)
                : null;

            if (step == null)
            {
                step = new Step();
                instruction.Steps.Add(step);
            }

            step.Title = item.Title!.Trim();
            step.Description = item.Description?.Trim() ?? string.Empty;
            step.Order = index + 1;
        }

        _db.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    // Кнопка «Удалить»
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var instruction = _db.Instructions
            .FirstOrDefault(i => i.Id == id && i.AuthorId == CurrentAuthorId);

        if (instruction == null)
            return NotFound();

        _db.Instructions.Remove(instruction);
        _db.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
    // Добавляет пустые поля, чтобы всегда было 10 штук
    private void PrepareForm(InstructionFormViewModel model)
    {
        while(model.Steps.Count < StepSlots)
            model.Steps.Add(new StepFormItem());

        ViewBag.Categories = _db.Instructions
            .Select(i => i.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

    }
    public IActionResult Analytics(int id)
    {
        // Смотреть аналитику может только автор своей инструкции
        var instruction = _db.Instructions
            .AsNoTracking()
            .Include(i => i.Steps)
            .FirstOrDefault(i => i.Id == id && i.AuthorId == CurrentAuthorId);

        if (instruction == null)
            return NotFound();

        var started = _db.Progresses.Count(p => p.InstructionId == id);
        var completed = _db.Progresses.Count(p => p.InstructionId == id && p.IsCompleted);

        // Для каждого шага: сколько человек его отметили
        var doneByStep = _db.ProgressSteps
            .Where(ps => ps.Progress!.InstructionId == id)
            .GroupBy(ps => ps.StepId)
            .Select(g => new { StepId = g.Key, Count = g.Count() })
            .ToDictionary(x => x.StepId, x => x.Count);

        var steps = new List<StepAnalyticsViewModel>();
        var previous = started;   // для первого шага сравниваем с числом начавших

        foreach (var s in instruction.Steps.OrderBy(s => s.Order))
        {
            var done = doneByStep.TryGetValue(s.Id, out var count) ? count : 0;

            steps.Add(new StepAnalyticsViewModel
            {
                Order = s.Order,
                Title = s.Title,
                DoneCount = done,
                PercentOfStarted = started == 0 ? 0 : done * 100 / started,
                DropFromPrevious = Math.Max(0, previous - done)
            });

            previous = done;
        }

        // Узкое место: шаг с самым большим падением
        var maxDrop = steps.Count == 0 ? 0 : steps.Max(s => s.DropFromPrevious);
        if (maxDrop > 0)
            steps.First(s => s.DropFromPrevious == maxDrop).IsBottleneck = true;

        var ratings = _db.Ratings.Where(r => r.InstructionId == id);
        var ratingsCount = ratings.Count();

        var model = new InstructionAnalyticsViewModel
        {
            InstructionId = instruction.Id,
            Title = instruction.Title,
            StartedCount = started,
            CompletedCount = completed,
            RatingsCount = ratingsCount,
            AverageRating = ratingsCount == 0 ? 0 : ratings.Average(r => (double)r.Value),
            Steps = steps
        };

        return View(model);
    }
}
