using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StepWise.Data;
using StepWise.Models;
using StepWise.Services;
using StepWise.ViewModels;

namespace StepWise.Controllers;

public class PlayerController : Controller
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _currentUser;

    public PlayerController(AppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    // /Player?instructionId=1
    public IActionResult Index(int instructionId)
    {
        var instruction = _db.Instructions
            .AsNoTracking()
            .Include(i => i.Steps)
            .FirstOrDefault(i => i.Id == instructionId);

        if (instruction == null)
            return NotFound();

        var userId = _currentUser.UserId;

        // Ищем прохождение этого человека; если нет, создаём
        var progress = _db.Progresses
            .Include(p => p.ProgressSteps)
            .FirstOrDefault(p => p.InstructionId == instructionId && p.UserId == userId);

        if (progress == null)
        {
            progress = new Progress { InstructionId = instructionId, UserId = userId };
            _db.Progresses.Add(progress);
            _db.SaveChanges();
        }

        var doneIds = progress.ProgressSteps.Select(ps => ps.StepId).ToHashSet();

        var model = new PlayerViewModel
        {
            InstructionId = instruction.Id,
            Title = instruction.Title,
            IsCompleted = progress.IsCompleted,
            Steps = instruction.Steps
                .OrderBy(s => s.Order)
                .Select(s => new PlayerStepViewModel
                {
                    Id = s.Id,
                    Order = s.Order,
                    Title = s.Title,
                    Description = s.Description,
                    IsDone = doneIds.Contains(s.Id)
                })
                .ToList()
        };

        return View(model);
    }

    // Вызывается из JavaScript при клике на галочку
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ToggleStep(int instructionId, int stepId)
    {
        var userId = _currentUser.UserId;

        // Шаг должен действительно относиться к этой инструкции
        if (!_db.Steps.Any(s => s.Id == stepId && s.InstructionId == instructionId))
            return NotFound();

        var progress = _db.Progresses
            .Include(p => p.ProgressSteps)
            .FirstOrDefault(p => p.InstructionId == instructionId && p.UserId == userId);

        if (progress == null)
            return NotFound();

        var existing = progress.ProgressSteps.FirstOrDefault(ps => ps.StepId == stepId);
        bool isDone;

        if (existing == null)
        {
            progress.ProgressSteps.Add(new ProgressStep { StepId = stepId });
            isDone = true;
        }
        else
        {
            _db.ProgressSteps.Remove(existing);
            isDone = false;
        }
        _db.SaveChanges();

        // Пересчитываем по базе
        var total = _db.Steps.Count(s => s.InstructionId == instructionId);
        var done = _db.ProgressSteps.Count(ps => ps.ProgressId == progress.Id);

        progress.IsCompleted = total > 0 && done == total;
        _db.SaveChanges();

        return Json(new
        {
            isDone,
            done,
            total,
            percent = total == 0 ? 0 : done * 100 / total,
            isCompleted = progress.IsCompleted
        });
    }
}