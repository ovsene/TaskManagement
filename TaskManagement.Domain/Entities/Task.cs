using System;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities
{
    public class Task
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public TaskManagement.Domain.Enums.TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        
        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; }
        
        public Guid AssignedToId { get; set; }
        public User AssignedTo { get; set; }
        
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
    }
} 