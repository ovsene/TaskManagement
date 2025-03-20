using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Application.Users.Queries.GetUsers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, BaseResponse<List<UserDto>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetUsersQueryHandler> _logger;

        public GetUsersQueryHandler(ApplicationDbContext context, ILogger<GetUsersQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BaseResponse<List<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving all users");

                var users = await _context.Users
                    .Include(u => u.Department)
                    .ToListAsync(cancellationToken);

                var userDtos = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department?.Name
                }).ToList();

                _logger.LogInformation("Successfully retrieved {Count} users", userDtos.Count);
                return BaseResponse<List<UserDto>>.CreateSuccess(userDtos, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return BaseResponse<List<UserDto>>.CreateError($"Error retrieving users: {ex.Message}");
            }
        }
    }
} 