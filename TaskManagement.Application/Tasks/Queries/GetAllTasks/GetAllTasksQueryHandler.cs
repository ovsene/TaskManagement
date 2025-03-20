using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Domain.Common.Interfaces;

namespace TaskManagement.Application.Tasks.Queries.GetAllTasks
{
    public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, BaseResponse<List<TaskDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetAllTasksQueryHandler> _logger;

        public GetAllTasksQueryHandler(IApplicationDbContext context, ILogger<GetAllTasksQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BaseResponse<List<TaskDto>>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving all tasks");

                var tasks = await _context.Tasks
                    .Include(t => t.CreatedBy)
                    .Include(t => t.AssignedTo)
                    .Include(t => t.Department)
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
                        CompletedDate=t.CompletedDate,
                        Priority = t.Priority,
                        AssignedToId = t.AssignedToId,
                        AssignedToName = t.AssignedTo.Name,
                        DepartmentId = t.DepartmentId,
                        DepartmentName = t.Department.Name
                    })
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Retrieved {Count} tasks", tasks.Count);
                return BaseResponse<List<TaskDto>>.CreateSuccess(tasks, "Tasks retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                return BaseResponse<List<TaskDto>>.CreateError("Error retrieving tasks: " + ex.Message);
            }
        }
    }
} 