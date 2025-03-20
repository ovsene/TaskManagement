using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Users.Commands.Login;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.API.Extensions;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<BaseResponse<LoginResponseDto>>> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (result.Success)
            {
                // Store user information in session
                HttpContext.Session.SetUserId(result.Data.User.Id);
                HttpContext.Session.SetUserEmail(result.Data.User.Email);
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out successfully" });
        }
    }
} 