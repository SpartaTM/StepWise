
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StepWise.Data;
using StepWise.Models;
using StepWise.Services;
using StepWise.ViewModels;

namespace StepWise.Controllers;

public class InstructionsController : Controller
{
    private readonly ICurrentUser _currentUser;
    private readonly AppDbContext _db;

    // ASP.NET сам передаёт сюда базу данных
    public InstructionsController(AppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    // /Instructions — каталог
    public IActionResult Index(string? category, string? sort, string? search)
    {
        var query = _db.Instructions.AsNoTracking();

        // Поиск по названию и описанию
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(i =>
                i.Title.Contains(search) ||
                i.Description.Contains(search));
        }

        // Если категория выбрана, оставляем только её
        if (!string.IsNullOrEmpty(category))
            query = query.Where(i => i.Category == category);

        var items = query
            .Select(i => new InstructionListItemViewModel
            {
                Id = i.Id,
                Title = i.Title,
                Category = i.Category,
                StepsCount = i.Steps.Count,
                RatingsCount = i.Ratings.Count,
                AverageRating = i.Ratings.Any()
                    ? i.Ratings.Average(r => (double)r.Value)
                    : 0
            })
            .ToList();

        // Сортировка по рейтингу
        if (sort == "rating")
        {
            items = items
                .OrderByDescending(i => i.AverageRating)
                .ThenByDescending(i => i.RatingsCount)
                .ToList();
        }

        // Список всех категорий для кнопок фильтра
        ViewBag.Categories = _db.Instructions
            .Select(i => i.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        ViewBag.SelectedCategory = category;
        ViewBag.Search = search;
        ViewBag.Sort = sort;

        return View(items);
    }

    // /Instructions/Details/5 — одна инструкция
    public IActionResult Details(int id)
    {
        var instruction = _db.Instructions
            .Include(i => i.Steps)
            .FirstOrDefault(i => i.Id == id);

        if (instruction == null)
            return NotFound();

        var ratings = _db.Ratings.Where(r => r.InstructionId == id);
        var ratingsCount = ratings.Count();
        var average = ratingsCount == 0
            ? 0
            : ratings.Average(r => (double)r.Value);

        var myRating = ratings
            .Where(r => r.UserId == _currentUser.UserId)
            .Select(r => (int?)r.Value)
            .FirstOrDefault();

        string? forkedFromTitle = null;
        if (instruction.ForkedFromId != null)
        {
            forkedFromTitle = _db.Instructions
                .Where(i => i.Id == instruction.ForkedFromId)
                .Select(i => i.Title)
                .FirstOrDefault();
        }

        var forksCount = _db.Instructions.Count(i => i.ForkedFromId == id);

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
                .ToList(),
            RatingsCount = ratingsCount,
            AverageRating = average,
            MyRating = myRating,
            ForkedFromId = forkedFromTitle == null ? null : instruction.ForkedFromId,
            ForkedFromTitle = forkedFromTitle,
            ForksCount = forksCount,
            IsMine = instruction.AuthorId == _currentUser.UserId
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Rate(int id, int value)
    {
        if (value < 1 || value > 5)
            return BadRequest();

        if (!_db.Instructions.Any(i => i.Id == id))
            return NotFound();

        var userId = _currentUser.UserId;

        var rating = _db.Ratings
            .FirstOrDefault(r =>
                r.InstructionId == id &&
                r.UserId == userId);

        if (rating == null)
        {
            _db.Ratings.Add(new Rating
            {
                InstructionId = id,
                UserId = userId,
                Value = value
            });
        }
        else
        {
            rating.Value = value;
            rating.CreatedAt = DateTime.UtcNow;
        }

        _db.SaveChanges();

        return RedirectToAction(nameof(Details), new { id });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Fork(int id)
    {
        var original = _db.Instructions
            .AsNoTracking()
            .Include(i => i.Steps)
            .FirstOrDefault(i => i.Id == id);

        if (original == null)
            return NotFound();

        var userId = _currentUser.UserId;

        if (original.AuthorId == userId)
            return RedirectToAction("Edit", "Author", new { id });

        var title = original.Title.Length > 90 ? original.Title[..90] : original.Title;

        var copy = new Instruction
        {
            Title = title + " (копия)",
            Description = original.Description,
            Category = original.Category,
            AuthorId = userId,
            ForkedFromId = original.Id,
            Steps = original.Steps
                .OrderBy(s => s.Order)
                .Select(s => new Step
                {
                    Order = s.Order,
                    Title = s.Title,
                    Description = s.Description
                })
                .ToList()
        };

        _db.Instructions.Add(copy);
        _db.SaveChanges();

        return RedirectToAction("Edit", "Author", new { id = copy.Id });
    }
}
