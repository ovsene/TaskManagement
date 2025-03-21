using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Domain.Common.Interfaces;

namespace TaskManagement.Application.Tasks.Queries.GetTaskById
{
    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, BaseResponse<TaskDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetTaskByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BaseResponse<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var task = await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (task == null)
                return BaseResponse<TaskDto>.CreateError("Task not found");

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

            return BaseResponse<TaskDto>.CreateSuccess(taskDto, "Task retrieved successfully");
        }
    }
} 