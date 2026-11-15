using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StepWise.Data;
using StepWise.ViewModels;

namespace StepWise.Controllers;

public class InstructionsController : Controller
{
    private readonly AppDbContext _db;

    // ASP.NET сам передаёт сюда базу данных
    public InstructionsController(AppDbContext db)
    {
        _db = db;
    }

    // /Instructions  — каталог
    public IActionResult Index()
    {
        var items = _db.Instructions
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

    // /Instructions/Details/5  — одна инструкция
    public IActionResult Details(int id)
    {
        var instruction = _db.Instructions
            .Include(i => i.Steps)
            .FirstOrDefault(i => i.Id == id);

        if (instruction == null)
            return NotFound();

        var model = new InstructionDetailsViewModel
        {
            Id = instruction.Id,
            Title = instruction.Title,
            Description = instruction.Description,
            Category = instruction.Category,
            Steps = instruction.Steps
                .OrderBy(s => s.Order)
                .Select(s => new StepViewModel
                {
                    Id = s.Id,
                    Order = s.Order,
                    Title = s.Title,
                    Description = s.Description
                })
                .ToList()
        };

        return View(model);
    }
}