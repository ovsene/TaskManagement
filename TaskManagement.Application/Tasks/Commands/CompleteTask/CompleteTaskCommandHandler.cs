using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Domain.Common.Interfaces;

namespace TaskManagement.Application.Tasks.Commands.CompleteTask
{
    public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, BaseResponse<TaskDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CompleteTaskCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public CompleteTaskCommandHandler(
            IApplicationDbContext context,
            ILogger<CompleteTaskCommandHandler> logger,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<BaseResponse<TaskDto>> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to complete task with ID: {TaskId}", request.Id);

                var currentUserId = _currentUserService.UserId;
                if (string.IsNullOrEmpty(currentUserId.ToString()))
                {
                    _logger.LogWarning("No user ID found in current session");
                    return BaseResponse<TaskDto>.CreateError("User not authenticated");
                }
                var user = await _context.Users
                   .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
                var task = await _context.Tasks
                    .Include(t => t.CreatedBy)
                    .Include(t => t.AssignedTo)
                    .Include(t => t.Department)
                    .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

                if (task == null)
                {
                    _logger.LogWarning("Task with ID {TaskId} not found", request.Id);
                    return BaseResponse<TaskDto>.CreateError("Task not found");
                }

                if (task.DepartmentId != user.DepartmentId)
                {
                    _logger.LogWarning("User {UserId} from department {UserDepartmentId} attempted to reject task {TaskId} from department {TaskDepartmentId}",
                        currentUserId, user.DepartmentId, request.Id, task.DepartmentId);
                    return BaseResponse<TaskDto>.CreateError("You can only reject tasks assigned to your department");
                }

                task.Status = TaskManagement.Domain.Enums.TaskStatus.Completed;
                task.CompletedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Task {TaskId} completed successfully", request.Id);

                var taskDto = new TaskDto
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
                };

                return BaseResponse<TaskDto>.CreateSuccess(taskDto, "Task completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing task {TaskId}", request.Id);
                return BaseResponse<TaskDto>.CreateError($"Error completing task: {ex.Message}");
            }
        }
    }
} 