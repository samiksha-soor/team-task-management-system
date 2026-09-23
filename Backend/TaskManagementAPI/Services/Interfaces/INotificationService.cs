using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifyTaskAssignedAsync(TaskItem task);
        Task NotifyTaskStatusUpdatedAsync(TaskItem task);
        Task<IEnumerable<NotificationResponseDto>> GetMyNotificationsAsync(int userId);
        Task MarkAsReadAsync(int id, int userId);
    }
}
