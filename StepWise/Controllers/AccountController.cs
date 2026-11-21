using Microsoft.AspNetCore.Mvc;
using StepWise.Services;

namespace StepWise.Controllers;

public class AccountController : Controller
{
    // Нажата кнопка переключения роли
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetRole(string role, string? returnUrl)
    {
        role = role == "expert" ? "expert" : "student";

        Response.Cookies.Append(CurrentUser.RoleCookie, role, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });

        // Возвращаем на ту же страницу, но только внутри нашего сайта
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "Instructions");
    }

    // Страница для ученика, попавшего на страницу эксперта
    public IActionResult ExpertOnly(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
}