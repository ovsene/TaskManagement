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
            var userId = context.Session.GetUserId();
            if (userId.HasValue)
            {
                context.Items["CurrentUser"] = new UserContext
                {
                    UserId = userId.Value,
                    Email = context.Session.GetUserEmail()
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

    // Extension method to make it easier to add the middleware
    public static class UserContextMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserContext(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserContextMiddleware>();
        }
    }
} 