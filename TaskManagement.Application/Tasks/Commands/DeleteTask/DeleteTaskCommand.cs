using MediatR;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;

namespace TaskManagement.Application.Tasks.Commands.DeleteTask
{
    public class DeleteTaskCommand : IRequest<BaseResponse<Unit>>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }
} 