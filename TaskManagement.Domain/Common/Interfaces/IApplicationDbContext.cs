using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Domain.Entities.Task> Tasks { get; set; }
        DbSet<Department> Departments { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
} 