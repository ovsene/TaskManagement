using System;
using System.Collections.Generic;

namespace TaskManagement.Domain.Entities
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
} 