using TaskManagement.API.Extensions;

namespace TaskManagement.API.Middleware
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userClaims = context.User.Claims;
            var userIdClaim = userClaims.FirstOrDefault(c => c.Type == "userId");
            var emailClaim = userClaims.FirstOrDefault(c => c.Type == "email");
            if (userIdClaim != null && string.IsNullOrEmpty(userIdClaim?.ToString()))
            {
                context.Items["CurrentUser"] = new UserContext
                {
                    UserId = Guid.Parse(userIdClaim?.ToString().Split(":")[1]),
                    Email = emailClaim?.ToString().Split(":")[1]
                };
            }

            await _next(context);
        }
    }

    public class UserContext
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
    }
    public static class UserContextMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserContext(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserContextMiddleware>();
        }
    }
}