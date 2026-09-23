using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class TaskComment
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
