using MediatR;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;

namespace TaskManagement.Application.Tasks.Queries.GetAllTasks
{
    public class GetAllTasksQuery : IRequest<BaseResponse<List<TaskDto>>>
    {
    }
} 