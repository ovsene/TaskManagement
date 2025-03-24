using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Domain.Common.Interfaces;

namespace TaskManagement.Application.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, BaseResponse<TaskDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdateTaskCommandHandler> _logger;

        public UpdateTaskCommandHandler(IApplicationDbContext context, ILogger<UpdateTaskCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BaseResponse<TaskDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating task with ID: {TaskId}", request.Id);

                var assignedTo = await _context.Users.FindAsync(request.AssignedToId);
                if (assignedTo == null)
                {
                    _logger.LogWarning("Assigned user with ID {UserId} not found", request.AssignedToId);
                    return BaseResponse<TaskDto>.CreateError("Assigned user not found");
                }

                var department = await _context.Departments.FindAsync(request.DepartmentId);
                if (department == null)
                {
                    _logger.LogWarning("Department with ID {DepartmentId} not found", request.DepartmentId);
                    return BaseResponse<TaskDto>.CreateError("Department not found");
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

                if (task.CreatedById != request.UserId)
                {
                    _logger.LogWarning("User {UserId} attempted to update task {TaskId} created by {CreatedById}",
                        request.UserId, request.Id, task.CreatedById);
                    return BaseResponse<TaskDto>.CreateError("You can only update tasks that you created");
                }

                task.Title = request.Title;
                task.Description = request.Description;
                task.DueDate = request.DueDate;
                task.AssignedToId = request.AssignedToId;
                task.DepartmentId = request.DepartmentId;
                task.Priority = request.Priority;

                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Task {TaskId} updated successfully", task.Id);

                var updatedTask = await _context.Tasks
                    .Include(t => t.CreatedBy)
                    .Include(t => t.AssignedTo)
                    .Include(t => t.Department)
                    .FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken);

                if (updatedTask == null)
                {
                    _logger.LogWarning("Task {TaskId} not found after update", task.Id);
                    return BaseResponse<TaskDto>.CreateError("Task not found after update");
                }

                var taskDto = new TaskDto
                {
                    Id = updatedTask.Id,
                    Title = updatedTask.Title,
                    Description = updatedTask.Description,
                    CreatedDate = updatedTask.CreatedDate,
                    DueDate = updatedTask.DueDate,
                    Status = updatedTask.Status,
                    CreatedById = updatedTask.CreatedById,
                    CreatedByName = updatedTask.CreatedBy?.Name,
                    AssignedToId = updatedTask.AssignedToId,
                    AssignedToName = updatedTask.AssignedTo?.Name,
                    DepartmentId = updatedTask.DepartmentId,
                    DepartmentName = updatedTask.Department?.Name,
                    Priority=updatedTask.Priority,
                };

                return BaseResponse<TaskDto>.CreateSuccess(taskDto, "Task updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", request.Id);
                return BaseResponse<TaskDto>.CreateError($"Error updating task: {ex.Message}");
            }
        }
    }
}