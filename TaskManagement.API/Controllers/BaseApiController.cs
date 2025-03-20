using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common.Models;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        private ISender _mediator;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

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