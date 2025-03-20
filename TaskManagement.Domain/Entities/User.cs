using System;
using System.Collections.Generic;

namespace TaskManagement.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
        public ICollection<Task> CreatedTasks { get; set; }
        public ICollection<Task> AssignedTasks { get; set; }
    }
} 