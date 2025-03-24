using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManagement.API.Extensions;

namespace TaskManagement.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateUserIdAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userClaims = context.HttpContext.User.Claims;
            var userIdClaim = userClaims.FirstOrDefault(c => c.Type == "userId");

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.ToString()))
            {
                context.Result = new UnauthorizedObjectResult("User not authenticated");
                return;
            }

        }
    }
}