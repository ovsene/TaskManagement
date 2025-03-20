using MediatR;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;

namespace TaskManagement.Application.Tasks.Queries.GetTaskById
{
    public class GetTaskByIdQuery : IRequest<BaseResponse<TaskDto>>
    {
        public Guid Id { get; set; }
    }
} 