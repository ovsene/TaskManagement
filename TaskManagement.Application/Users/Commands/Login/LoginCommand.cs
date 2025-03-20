using MediatR;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Users.DTOs;

namespace TaskManagement.Application.Users.Commands.Login
{
    public class LoginCommand : IRequest<BaseResponse<LoginResponseDto>>
    {
        public string Email { get; set; }
    }
} 