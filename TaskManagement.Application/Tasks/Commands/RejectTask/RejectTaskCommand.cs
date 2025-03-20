using MediatR;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;

namespace TaskManagement.Application.Tasks.Commands.RejectTask
{
    public class RejectTaskCommand : IRequest<BaseResponse<TaskDto>>
    {
        public Guid Id { get; set; }
    }
} 