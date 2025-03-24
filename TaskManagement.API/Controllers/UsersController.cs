using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Attributes;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Users.Commands.Login;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Application.Users.Queries.GetUsers;
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

        [HttpGet]
        [ValidateUserId]
        public async Task<ActionResult<BaseResponse<List<UserDto>>>> GetUsers()
        {
            var result = await _mediator.Send(new GetUsersQuery());
            return Ok(result);
        }
    }
} 