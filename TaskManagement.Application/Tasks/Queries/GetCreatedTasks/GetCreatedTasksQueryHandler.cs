using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Domain.Common.Interfaces;

namespace TaskManagement.Application.Tasks.Queries.GetCreatedTasks
{
    public class GetCreatedTasksQueryHandler : IRequestHandler<GetCreatedTasksQuery, BaseResponse<List<TaskDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetCreatedTasksQueryHandler> _logger;

        public GetCreatedTasksQueryHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            ILogger<GetCreatedTasksQueryHandler> logger)
        {
            _context = context;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<BaseResponse<List<TaskDto>>> Handle(GetCreatedTasksQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching tasks created by user: {UserId}", _currentUserService.UserId);

                var tasks = await _context.Tasks
                    .Include(t => t.CreatedBy)
                    .Include(t => t.AssignedTo)
                    .Include(t => t.Department)
                    .Where(t => t.CreatedById == _currentUserService.UserId)
                    .OrderByDescending(t => t.CreatedDate)
                    .ToListAsync(cancellationToken);

                var taskDtos = tasks.Select(task => new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    CreatedDate = task.CreatedDate,
                    DueDate = task.DueDate,
                    CompletedDate = task.CompletedDate,
                    Status = task.Status,
                    Priority = task.Priority,
                    CreatedById = task.CreatedById,
                    CreatedByName = task.CreatedBy?.Name,
                    AssignedToId = task.AssignedToId,
                    AssignedToName = task.AssignedTo?.Name,
                    DepartmentId = task.DepartmentId,
                    DepartmentName = task.Department?.Name
                }).ToList();

                _logger.LogInformation("Successfully retrieved {Count} tasks created by user {UserId}", 
                    taskDtos.Count, _currentUserService.UserId);

                return BaseResponse<List<TaskDto>>.CreateSuccess(taskDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks created by user {UserId}", _currentUserService.UserId);
                return BaseResponse<List<TaskDto>>.CreateError($"Error retrieving created tasks: {ex.Message}");
            }
        }
    }
} 