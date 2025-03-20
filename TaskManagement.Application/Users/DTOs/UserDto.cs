namespace TaskManagement.Application.Users.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
} 