using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.ToDo;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public DateTime? Deadline { get; set; }

        public int TeamId { get; set; }
        public TeamEntity? Team { get; set; }

        public int AssignedToId { get; set; }
        public User? AssignedTo { get; set; }

        public int CreatedById { get; set; }
        public User? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }
}
