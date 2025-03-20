using MediatR;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;

namespace TaskManagement.Application.Tasks.Commands.CompleteTask
{
    public class CompleteTaskCommand : IRequest<BaseResponse<TaskDto>>
    {
        public Guid Id { get; set; }
    }
} 