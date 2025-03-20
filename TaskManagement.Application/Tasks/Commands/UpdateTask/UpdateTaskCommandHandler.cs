using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Application.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, BaseResponse<TaskDto>>
    {
        private readonly ApplicationDbContext _context;

        public UpdateTaskCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BaseResponse<TaskDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (task == null)
                return BaseResponse<TaskDto>.CreateError("Task not found");

            if (task.CreatedById != request.UserId)
                return BaseResponse<TaskDto>.CreateError("You can only update tasks that you created");

            task.Title = request.Title;
            task.Description = request.Description;
            task.DueDate = request.DueDate;
            task.AssignedToId = request.AssignedToId;
            task.DepartmentId = request.DepartmentId;

            await _context.SaveChangesAsync(cancellationToken);

            var taskDto = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CreatedDate = task.CreatedDate,
                DueDate = task.DueDate,
                Status = task.Status,
                CreatedById = task.CreatedById,
                CreatedByName = task.CreatedBy.Name,
                AssignedToId = task.AssignedToId,
                AssignedToName = task.AssignedTo.Name,
                DepartmentId = task.DepartmentId,
                DepartmentName = task.Department.Name
            };

            return BaseResponse<TaskDto>.CreateSuccess(taskDto, "Task updated successfully");
        }
    }
} 