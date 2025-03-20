using System;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Tasks.DTOs
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public Domain.Enums.TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public Guid AssignedToId { get; set; }
        public string AssignedToName { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
} 