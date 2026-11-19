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
    // Временно. На 19.11 заменим на значение из куки
    private string CurrentAuthorId => _currentUser.UserId;
    private const int StepSlots = 10;

    public AuthorController(AppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    // /Author — список моих инструкций
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

    // /Author/Create — показать пустую форму
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
    // /Author/Edit/5 — форма с текущими данными
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

        // 1. Удаляем шаги, которые автор очистил в форме
        var keepIds = filledSteps
            .Where(s => s.Id.HasValue)
            .Select(s => s.Id!.Value)
            .ToList();
        var toRemove = instruction.Steps.Where(s => !keepIds.Contains(s.Id)).ToList();
        _db.Steps.RemoveRange(toRemove);

        // 2. Обновляем существующие шаги и добавляем новые
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
}
