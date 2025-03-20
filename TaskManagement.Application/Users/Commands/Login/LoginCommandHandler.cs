using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Common.Services;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Domain.Common.Interfaces;

namespace TaskManagement.Application.Users.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseResponse<LoginResponseDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IApplicationDbContext context,
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
                _logger.LogInformation("Login attempt for user: {Email}", request.Email);

                var user = await _context.Users
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found - {Email}", request.Email);
                    return BaseResponse<LoginResponseDto>.CreateError("Invalid email");
                }

                var token = _jwtService.GenerateToken(user);
                _logger.LogInformation("Login successful for user: {Email}", request.Email);

                return BaseResponse<LoginResponseDto>.CreateSuccess(new LoginResponseDto
                {
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        DepartmentId = user.DepartmentId,
                        DepartmentName = user.Department?.Name
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Email}", request.Email);
                return BaseResponse<LoginResponseDto>.CreateError("An error occurred during login");
            }
        }
    }
}