using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Application.Tasks.Queries.GetUserTasks
{
    public class GetUserTasksQueryHandler : IRequestHandler<GetUserTasksQuery, BaseResponse<List<TaskDto>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public GetUserTasksQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<BaseResponse<List<TaskDto>>> Handle(GetUserTasksQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var tasks = await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Department)
                .Where(t => t.CreatedById == currentUserId)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    CreatedDate = t.CreatedDate,
                    DueDate = t.DueDate,
                    Status = t.Status,
                    CreatedById = t.CreatedById,
                    CreatedByName = t.CreatedBy.Name,
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo.Name,
                    DepartmentId = t.DepartmentId,
                    DepartmentName = t.Department.Name
                })
                .ToListAsync(cancellationToken);

            return BaseResponse<List<TaskDto>>.CreateSuccess(tasks, "User tasks retrieved successfully");
        }
    }
} 