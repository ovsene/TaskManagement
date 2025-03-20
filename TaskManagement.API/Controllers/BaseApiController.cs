using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common.Models;
using TaskManagement.API.Middleware;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        private ISender _mediator;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        protected UserContext CurrentUser => HttpContext.Items["CurrentUser"] as UserContext;

        protected Guid GetCurrentUserId()
        {
            if (CurrentUser == null)
                throw new UnauthorizedAccessException("User is not authenticated");
            
            return CurrentUser.UserId;
        }

        protected string GetCurrentUserEmail()
        {
            if (CurrentUser == null)
                throw new UnauthorizedAccessException("User is not authenticated");
            
            return CurrentUser.Email;
        }

        protected ActionResult<BaseResponse<T>> HandleResult<T>(BaseResponse<T> result)
        {
            if (result == null)
                return NotFound();

            if (result.Success && result.Data == null)
                return NotFound();

            if (result.Success && result.Data != null)
                return Ok(result);

            return BadRequest(result);
        }
    }
} 