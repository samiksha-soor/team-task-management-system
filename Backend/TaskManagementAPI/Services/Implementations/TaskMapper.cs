using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services.Implementations
{
    public static class TaskMapper
    {
        public static TaskResponseDto ToDto(TaskItem t) => new()
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status.ToString(),
            Priority = t.Priority.ToString(),
            Deadline = t.Deadline,
            TeamId = t.TeamId,
            TeamName = t.Team?.Name ?? string.Empty,
            AssignedToId = t.AssignedToId,
            AssignedToName = t.AssignedTo?.FullName ?? string.Empty,
            CreatedById = t.CreatedById,
            CreatedByName = t.CreatedBy?.FullName ?? string.Empty,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        };
    }
}
