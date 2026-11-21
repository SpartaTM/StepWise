namespace StepWise.Services;

public interface ICurrentUser
{
    string UserId { get; }
    string Role { get; }      // "student" или "expert"
    bool IsExpert { get; }
}

public class CurrentUser : ICurrentUser
{
    public const string UserCookie = "stepwise_uid";
    public const string RoleCookie = "stepwise_role";

    public string UserId { get; }
    public string Role { get; }
    public bool IsExpert => Role == "expert";

    public CurrentUser(IHttpContextAccessor accessor)
    {
        var context = accessor.HttpContext!;

        // Роль: если кука не выставлена, человек считается учеником
        Role = context.Request.Cookies.TryGetValue(RoleCookie, out var role) && role == "expert"
            ? "expert"
            : "student";

        // Идентификатор пользователя, как раньше
        if (context.Request.Cookies.TryGetValue(UserCookie, out var id)
            && Guid.TryParse(id, out _))
        {
            UserId = id;
            return;
        }

        UserId = Guid.NewGuid().ToString();
        context.Response.Cookies.Append(UserCookie, UserId, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });
    }
}