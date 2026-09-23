using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories.Implementations;
using TaskManagementAPI.Services.Implementations;
using TaskManagementAPI.Tests.Helpers;
using Xunit;

namespace TaskManagementAPI.Tests.Services
{
    public class NotificationServiceTests
    {
        [Fact]
        public async Task NotifyTaskAssignedAsync_CreatesOneNotificationForAssignee()
        {
            var db = TestDbContextFactory.Create();
            var service = new NotificationService(new NotificationRepository(db), NullLogger<NotificationService>.Instance);

            var task = new TaskItem { Id = 1, Title = "Write report", AssignedToId = 10, CreatedById = 20 };

            await service.NotifyTaskAssignedAsync(task);

            db.Notifications.Should().ContainSingle(n =>
                n.UserId == 10 &&
                n.Type == NotificationType.TaskAssigned &&
                n.Message.Contains("Write report"));
        }

        [Fact]
        public async Task NotifyTaskStatusUpdatedAsync_WhenAssigneeAndCreatorDiffer_NotifiesBoth()
        {
            var db = TestDbContextFactory.Create();
            var service = new NotificationService(new NotificationRepository(db), NullLogger<NotificationService>.Instance);

            var task = new TaskItem { Id = 2, Title = "Fix bug", AssignedToId = 10, CreatedById = 20, Status = TaskStatusEnum.Done };

            await service.NotifyTaskStatusUpdatedAsync(task);

            db.Notifications.Should().HaveCount(2);
            db.Notifications.Should().Contain(n => n.UserId == 10);
            db.Notifications.Should().Contain(n => n.UserId == 20);
        }

        [Fact]
        public async Task NotifyTaskStatusUpdatedAsync_WhenAssigneeIsCreator_NotifiesOnlyOnce()
        {
            var db = TestDbContextFactory.Create();
            var service = new NotificationService(new NotificationRepository(db), NullLogger<NotificationService>.Instance);

            var task = new TaskItem { Id = 3, Title = "Self task", AssignedToId = 10, CreatedById = 10, Status = TaskStatusEnum.InProgress };

            await service.NotifyTaskStatusUpdatedAsync(task);

            db.Notifications.Should().ContainSingle();
        }
    }
}
