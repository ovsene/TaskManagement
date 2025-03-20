using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManagement.Application.Common.Services;

namespace TaskManagement.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateUserIdAttribute : TypeFilterAttribute
    {
        public ValidateUserIdAttribute() : base(typeof(ValidateUserIdFilter))
        {
        }

        private class ValidateUserIdFilter : IAuthorizationFilter
        {
            private readonly IJwtService _jwtService;

            public ValidateUserIdFilter(IJwtService jwtService)
            {
                _jwtService = jwtService;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    context.Result = new UnauthorizedObjectResult("No token provided");
                    return;
                }

                if (!_jwtService.ValidateToken(token))
                {
                    context.Result = new UnauthorizedObjectResult("Invalid token");
                    return;
                }

                var userIdFromToken = _jwtService.GetUserIdFromToken(token);
                if (!userIdFromToken.HasValue)
                {
                    context.Result = new UnauthorizedObjectResult("Invalid token: User ID not found");
                    return;
                }

                // Get the user ID from the route or query string
                var userIdFromRequest = context.RouteData.Values["userId"]?.ToString();
                if (string.IsNullOrEmpty(userIdFromRequest))
                {
                    context.Result = new BadRequestObjectResult("User ID not provided in request");
                    return;
                }

                if (!Guid.TryParse(userIdFromRequest, out Guid requestUserId))
                {
                    context.Result = new BadRequestObjectResult("Invalid user ID format");
                    return;
                }

                if (requestUserId != userIdFromToken.Value)
                {
                    context.Result = new ForbidResult("User ID in token does not match request");
                }
            }
        }
    }
} 