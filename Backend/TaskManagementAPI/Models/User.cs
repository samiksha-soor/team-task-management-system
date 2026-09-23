using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.User;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? TeamId { get; set; }
        public TeamEntity? Team { get; set; }

        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
        public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }
}
