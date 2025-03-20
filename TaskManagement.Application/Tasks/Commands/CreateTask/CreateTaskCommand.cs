using MediatR;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Tasks.Commands.CreateTask
{
    public class CreateTaskCommand : IRequest<BaseResponse<TaskDto>>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid CreatedById { get; set; }
        public Guid AssignedToId { get; set; }
    }
} 