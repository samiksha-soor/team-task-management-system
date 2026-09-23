using TaskManagementAPI.DTOs;

namespace TaskManagementAPI.Services.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskResponseDto>> GetTasksAsync(int currentUserId, string currentRole, TaskFilterDto filter);
        Task<TaskResponseDto> GetTaskAsync(int id, int currentUserId, string currentRole);
        Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, int currentUserId, string currentRole);
        Task UpdateTaskAsync(int id, UpdateTaskDto dto, int currentUserId, string currentRole);
        Task UpdateStatusAsync(int id, UpdateTaskStatusDto dto, int currentUserId, string currentRole);
        Task DeleteTaskAsync(int id, int currentUserId, string currentRole);
    }
}
