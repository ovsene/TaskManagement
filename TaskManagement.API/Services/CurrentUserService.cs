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
                var userContext = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as UserContext;
                if (userContext == null)
                    throw new UnauthorizedAccessException("User is not authenticated");
                
                return userContext.UserId;
            }
        }

        public string Email
        {
            get
            {
                var userContext = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as UserContext;
                if (userContext == null)
                    throw new UnauthorizedAccessException("User is not authenticated");
                
                return userContext.Email;
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