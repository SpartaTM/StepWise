namespace StepWise.Services
{
    public interface ICurrentUser
    {
        string UserId { get; }
    }

    public class CurrentUser : ICurrentUser
    {
        private const string CookieName = "stepwise_uid";
        public string UserId { get; }
        public CurrentUser(IHttpContextAccessor accessor)
        {
            var context = accessor.HttpContext!;
            if (context.Request.Cookies.TryGetValue(CookieName, out var id) && Guid.TryParse(id, out _))
            {
                UserId = id;
                return;
            }

            UserId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append(CookieName, UserId, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });
        }
    }
}
