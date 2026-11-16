
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StepWise.Data;
using StepWise.Models;

namespace StepWise.Controllers;

public class PlayerController : Controller
{
    private readonly AppDbContext _db;

    private const string DemoUserId = "demo-user";

    public PlayerController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Index(int instructionId)
    {
        var instruction = _db.Instructions
            .Include(i => i.Steps)
            .FirstOrDefault(i => i.Id == instructionId);

        if (instruction == null)
            return NotFound();

        var progress = _db.Progresses
            .Include(p => p.ProgressSteps)
            .FirstOrDefault(p =>
                p.InstructionId == instructionId &&
                p.UserId == DemoUserId);

        if (progress == null)
        {
            progress = new Progress
            {
                InstructionId = instructionId,
                UserId = DemoUserId
            };

            _db.Progresses.Add(progress);
            _db.SaveChanges();
        }

        var completedStepIds = progress.ProgressSteps
            .Select(ps => ps.StepId)
            .ToList();

        ViewBag.ProgressId = progress.Id;
        ViewBag.CompletedStepIds = completedStepIds;

        ViewBag.ProgressPercent = instruction.Steps.Count == 0
            ? 0
            : (int)Math.Round(
                (double)completedStepIds.Count / instruction.Steps.Count * 100);

        return View(instruction);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ToggleStep(int progressId, int stepId)
    {
        var progress = _db.Progresses
            .Include(p => p.ProgressSteps)
            .FirstOrDefault(p =>
                p.Id == progressId &&
                p.UserId == DemoUserId);

        if (progress == null)
            return NotFound();

        var step = _db.Steps.FirstOrDefault(s =>
            s.Id == stepId &&
            s.InstructionId == progress.InstructionId);

        if (step == null)
            return BadRequest();

        var completedStep = progress.ProgressSteps
            .FirstOrDefault(ps => ps.StepId == stepId);

        if (completedStep == null)
        {
            progress.ProgressSteps.Add(new ProgressStep
            {
                StepId = stepId
            });
        }
        else
        {
            progress.ProgressSteps.Remove(completedStep);
            _db.ProgressSteps.Remove(completedStep);
        }

        var totalSteps = _db.Steps.Count(s =>
            s.InstructionId == progress.InstructionId);

        progress.IsCompleted = totalSteps > 0 &&
            progress.ProgressSteps.Count == totalSteps;

        _db.SaveChanges();

        return RedirectToAction(
            nameof(Index),
            new { instructionId = progress.InstructionId });
    }
}
