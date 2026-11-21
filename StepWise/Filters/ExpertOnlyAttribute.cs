using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using StepWise.Services;

namespace StepWise.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ExpertOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
        if (user.IsExpert)
            return;

        // Вернуться после смены роли можно только на страницу, открываемую обычной ссылкой (GET)
        var request = context.HttpContext.Request;
        var returnUrl = request.Method == "GET" ? request.Path + request.QueryString : "/";

        context.Result = new RedirectToActionResult("ExpertOnly", "Account", new { returnUrl });
    }
}