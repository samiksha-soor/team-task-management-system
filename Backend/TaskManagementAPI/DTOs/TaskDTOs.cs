using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.DTOs
{
    public class CreateTaskDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? Deadline { get; set; }

        [Required]
        public int TeamId { get; set; }

        [Required]
        public int AssignedToId { get; set; }
    }

    public class UpdateTaskDto
    {
        [MaxLength(200)]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TaskPriority? Priority { get; set; }
        public DateTime? Deadline { get; set; }
        public int? AssignedToId { get; set; }
    }

    public class UpdateTaskStatusDto
    {
        [Required]
        public TaskStatusEnum Status { get; set; }
    }

    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? Deadline { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int AssignedToId { get; set; }
        public string AssignedToName { get; set; } = string.Empty;
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TaskFilterDto
    {
        public TaskStatusEnum? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public DateTime? DeadlineFrom { get; set; }
        public DateTime? DeadlineTo { get; set; }
    }
}
