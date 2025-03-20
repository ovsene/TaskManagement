using MediatR;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;

namespace TaskManagement.Application.Tasks.Queries.GetDepartmentTasks
{
    public class GetDepartmentTasksQuery : IRequest<BaseResponse<List<TaskDto>>>
    {
        public Guid DepartmentId { get; set; }
    }
} 