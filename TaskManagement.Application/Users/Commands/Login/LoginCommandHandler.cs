using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Common.Services;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Application.Users.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseResponse<LoginResponseDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            ApplicationDbContext context,
            IJwtService jwtService,
            ILogger<LoginCommandHandler> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<BaseResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting login for user with email: {Email}", request.Email);

                var user = await _context.Users
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found with email {Email}", request.Email);
                    return BaseResponse<LoginResponseDto>.CreateError("Invalid email");
                }


                var token = _jwtService.GenerateToken(user.Id, user.Email);
                _logger.LogInformation("Login successful for user {Email}", request.Email);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    DepartmentId = user.DepartmentId,
                    DepartmentName = user.Department?.Name
                };

                var response = new LoginResponseDto
                {
                    User = userDto,
                    Token = token
                };

                return BaseResponse<LoginResponseDto>.CreateSuccess(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Email}", request.Email);
                return BaseResponse<LoginResponseDto>.CreateError("Error during login: " + ex.Message);
            }
        }
    }
}