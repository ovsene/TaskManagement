using MediatR;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;

namespace TaskManagement.Application.Tasks.Queries.GetUserTasks
{
    public class GetUserTasksQuery : IRequest<BaseResponse<List<TaskDto>>>
    {
    }
} 