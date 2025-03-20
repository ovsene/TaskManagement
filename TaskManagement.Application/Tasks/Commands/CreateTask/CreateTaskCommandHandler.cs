using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Application.Tasks.Commands.CreateTask
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, BaseResponse<TaskDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateTaskCommandHandler> _logger;

        public CreateTaskCommandHandler(ApplicationDbContext context, ILogger<CreateTaskCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BaseResponse<TaskDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating new task with title: {Title}", request.Title);

                // Verify that the related entities exist
                var createdBy = await _context.Users.FindAsync(request.CreatedById);
                if (createdBy == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", request.CreatedById);
                    return BaseResponse<TaskDto>.CreateError("User not found");
                }

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

                var task = new TaskManagement.Domain.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    CreatedDate = DateTime.UtcNow,
                    DueDate = request.DueDate,
                    Status = TaskManagement.Domain.Enums.TaskStatus.Created,
                    CreatedById = request.CreatedById,
                    AssignedToId = request.AssignedToId,
                    DepartmentId = request.DepartmentId
                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Task created successfully with ID: {TaskId}", task.Id);

                var taskDto = await _context.Tasks
                    .Include(t => t.CreatedBy)
                    .Include(t => t.AssignedTo)
                    .Include(t => t.Department)
                    .Where(t => t.Id == task.Id)
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
                    .FirstOrDefaultAsync(cancellationToken);

                if (taskDto == null)
                {
                    _logger.LogWarning("Task with ID {TaskId} not found after creation", task.Id);
                    return BaseResponse<TaskDto>.CreateError("Task not found after creation");
                }

                _logger.LogInformation("Task retrieved successfully with ID: {TaskId}", task.Id);
                return BaseResponse<TaskDto>.CreateSuccess(taskDto, "Task created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return BaseResponse<TaskDto>.CreateError("Error creating task: " + ex.Message);
            }
        }
    }
} 