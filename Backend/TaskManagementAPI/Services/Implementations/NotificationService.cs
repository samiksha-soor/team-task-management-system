using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Exceptions;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories.Interfaces;
using TaskManagementAPI.Services.Interfaces;

namespace TaskManagementAPI.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository notificationRepository, ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task NotifyTaskAssignedAsync(TaskItem task)
        {
            var message = $"You have been assigned a new task: \"{task.Title}\".";
            await CreateNotificationAsync(task.AssignedToId, NotificationType.TaskAssigned, message, task.Id);
        }

        public async Task NotifyTaskStatusUpdatedAsync(TaskItem task)
        {
            var message = $"Task \"{task.Title}\" status changed to {task.Status}.";
            await CreateNotificationAsync(task.AssignedToId, NotificationType.TaskStatusUpdated, message, task.Id);
            if (task.CreatedById != task.AssignedToId)
                await CreateNotificationAsync(task.CreatedById, NotificationType.TaskStatusUpdated, message, task.Id);
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetMyNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.QueryByUser(userId).ToListAsync();
            return notifications.Select(n => new NotificationResponseDto
            {
                Id = n.Id,
                Type = n.Type.ToString(),
                Message = n.Message,
                TaskItemId = n.TaskItemId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            });
        }

        public async Task MarkAsReadAsync(int id, int userId)
        {
            var notification = await _notificationRepository.GetByIdForUserAsync(id, userId);
            if (notification == null) throw new NotFoundException("Notification not found.");

            notification.IsRead = true;
            await _notificationRepository.SaveChangesAsync();
        }

        private async Task CreateNotificationAsync(int userId, NotificationType type, string message, int taskId)
        {
            var notification = new Notification
            {
                UserId = userId,
                Type = type,
                Message = message,
                TaskItemId = taskId
            };
            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            _logger.LogInformation("[MOCK EMAIL] To UserId {UserId}: {Message}", userId, message);
        }
    }
}
