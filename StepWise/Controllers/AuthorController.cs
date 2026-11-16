using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StepWise.Data;
using StepWise.Models;
using StepWise.ViewModels;

namespace StepWise.Controllers;

public class AuthorController : Controller
{
    private readonly AppDbContext _db;

    // Временно. На 19.11 заменим на значение из куки
    private const string CurrentAuthorId = "guest";
    private const int StepSlots = 10;

    public AuthorController(AppDbContext db)
    {
        _db = db;
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
        FillEmptySteps(model);
        return View(model);
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
            FillEmptySteps(model);
            return View(model);   // показываем форму снова с ошибками
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

    // Добавляет пустые поля, чтобы всегда было 10 штук
    private static void FillEmptySteps(InstructionFormViewModel model)
    {
        while (model.Steps.Count < StepSlots)
            model.Steps.Add(new StepFormItem());
    }
}