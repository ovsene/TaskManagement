using MediatR;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;

namespace TaskManagement.Application.Tasks.Queries.GetCreatedTasks
{
    public class GetCreatedTasksQuery : IRequest<BaseResponse<List<TaskDto>>>
    {
    }
} 