using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Domain.Common.Interfaces;

namespace TaskManagement.Application.Tasks.Commands.RejectTask
{
    public class RejectTaskCommandHandler : IRequestHandler<RejectTaskCommand, BaseResponse<TaskDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<RejectTaskCommandHandler> _logger;

        public RejectTaskCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            ILogger<RejectTaskCommandHandler> logger)
        {
            _context = context;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<BaseResponse<TaskDto>> Handle(RejectTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to reject task with ID: {TaskId}", request.Id);

                var currentUserId = _currentUserService.UserId;
                if (string.IsNullOrEmpty(currentUserId.ToString()))
                {
                    _logger.LogWarning("No user ID found in current session");
                    return BaseResponse<TaskDto>.CreateError("User not authenticated");
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", currentUserId);
                    return BaseResponse<TaskDto>.CreateError("User not found");
                }

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

                task.Status = Domain.Enums.TaskStatus.Rejected;
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Task {TaskId} rejected successfully", request.Id);

                var taskDto = new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    CreatedDate = task.CreatedDate,
                    DueDate = task.DueDate,
                    Status = task.Status,
                    CreatedById = task.CreatedById,
                    CreatedByName = task.CreatedBy?.Name,
                    AssignedToId = task.AssignedToId,
                    AssignedToName = task.AssignedTo?.Name,
                    DepartmentId = task.DepartmentId,
                    DepartmentName = task.Department?.Name
                };

                return BaseResponse<TaskDto>.CreateSuccess(taskDto, "Task rejected successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting task {TaskId}", request.Id);
                return BaseResponse<TaskDto>.CreateError($"Error rejecting task: {ex.Message}");
            }
        }
    }
}