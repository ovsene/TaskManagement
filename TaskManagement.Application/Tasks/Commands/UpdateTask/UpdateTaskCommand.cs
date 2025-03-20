using MediatR;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommand : IRequest<BaseResponse<TaskDto>>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid UserId { get; set; }
        public Guid AssignedToId { get; set; }
        public Guid DepartmentId { get; set; }
    }
} 