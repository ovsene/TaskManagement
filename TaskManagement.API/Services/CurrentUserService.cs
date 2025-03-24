using Microsoft.AspNetCore.Http;
using TaskManagement.API.Middleware;
using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userIdClaim = userClaims.FirstOrDefault(c => c.Type == "userId");
                if (userIdClaim == null)
                    throw new UnauthorizedAccessException("User is not authenticated");

                return Guid.Parse(userIdClaim?.ToString().Split(":")[1]);
            }
        }

        public string Email
        {
            get
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var emailClaim = userClaims.FirstOrDefault(c => c.Type == "email");
                if (emailClaim == null)
                    throw new UnauthorizedAccessException("User is not authenticated");

                return emailClaim?.ToString().Split(":")[1];
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _httpContextAccessor.HttpContext?.Items["CurrentUser"] != null;
            }
        }
    }
} 