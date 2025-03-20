using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Application.Tasks.Commands.CompleteTask
{
    public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, BaseResponse<TaskDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompleteTaskCommandHandler> _logger;

        public CompleteTaskCommandHandler(ApplicationDbContext context, ILogger<CompleteTaskCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BaseResponse<TaskDto>> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Completing task with ID: {TaskId} by user: {UserId}", request.Id, request.UserId);

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
                    _logger.LogWarning("User {UserId} is not authorized to complete task {TaskId}", request.UserId, request.Id);
                    return BaseResponse<TaskDto>.CreateError("You are not authorized to complete this task");
                }

                task.Status = TaskManagement.Domain.Enums.TaskStatus.Completed;
                task.CompletedDate = DateTime.UtcNow;

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

                _logger.LogInformation("Task {TaskId} completed successfully", request.Id);
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