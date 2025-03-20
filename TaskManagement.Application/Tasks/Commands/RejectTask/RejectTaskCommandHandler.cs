using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Application.Tasks.Commands.RejectTask
{
    public class RejectTaskCommandHandler : IRequestHandler<RejectTaskCommand, BaseResponse<TaskDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RejectTaskCommandHandler> _logger;

        public RejectTaskCommandHandler(ApplicationDbContext context, ILogger<RejectTaskCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BaseResponse<TaskDto>> Handle(RejectTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Rejecting task with ID: {TaskId} by user: {UserId}", request.Id, request.UserId);

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

                if (task.AssignedToId != request.UserId)
                {
                    _logger.LogWarning("User {UserId} is not authorized to reject task {TaskId}", request.UserId, request.Id);
                    return BaseResponse<TaskDto>.CreateError("You are not authorized to reject this task");
                }

                task.Status = TaskManagement.Domain.Enums.TaskStatus.Rejected;
                // Store rejection reason in a note or additional field if needed

                await _context.SaveChangesAsync(cancellationToken);

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

                _logger.LogInformation("Task {TaskId} rejected successfully", request.Id);
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