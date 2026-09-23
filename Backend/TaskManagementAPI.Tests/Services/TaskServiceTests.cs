using FluentAssertions;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Exceptions;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories.Implementations;
using TaskManagementAPI.Services.Implementations;
using TaskManagementAPI.Services.Interfaces;
using TaskManagementAPI.Tests.Helpers;
using Xunit;

namespace TaskManagementAPI.Tests.Services
{
    public class TaskServiceTests
    {
        private class FakeNotificationService : INotificationService
        {
            public Task NotifyTaskAssignedAsync(TaskItem task) => Task.CompletedTask;
            public Task NotifyTaskStatusUpdatedAsync(TaskItem task) => Task.CompletedTask;
            public Task<IEnumerable<NotificationResponseDto>> GetMyNotificationsAsync(int userId) =>
                Task.FromResult(Enumerable.Empty<NotificationResponseDto>());
            public Task MarkAsReadAsync(int id, int userId) => Task.CompletedTask;
        }

        private static TaskService CreateService(out ApplicationDbContext db)
        {
            db = TestDbContextFactory.Create();

            var admin = new User { Id = 1, FullName = "Alice Admin", Email = "alice@x.com", Role = UserRole.Admin, PasswordHash = "x" };
            var manager1 = new User { Id = 2, FullName = "Mo Manager", Email = "mo@x.com", Role = UserRole.Manager, PasswordHash = "x" };
            var manager2 = new User { Id = 3, FullName = "Rae Manager", Email = "rae@x.com", Role = UserRole.Manager, PasswordHash = "x" };
            var user1 = new User { Id = 4, FullName = "Uma User", Email = "uma@x.com", Role = UserRole.User, PasswordHash = "x", TeamId = 1 };
            var user2 = new User { Id = 5, FullName = "Zed User", Email = "zed@x.com", Role = UserRole.User, PasswordHash = "x", TeamId = 2 };
            db.Users.AddRange(admin, manager1, manager2, user1, user2);

            var team1 = new TeamEntity { Id = 1, Name = "Team One", ManagerId = 2 };
            var team2 = new TeamEntity { Id = 2, Name = "Team Two", ManagerId = 3 };
            db.Teams.AddRange(team1, team2);

            db.TaskItems.AddRange(
                new TaskItem { Id = 1, Title = "Task for team 1", TeamId = 1, AssignedToId = 4, CreatedById = 2 },
                new TaskItem { Id = 2, Title = "Task for team 2", TeamId = 2, AssignedToId = 5, CreatedById = 3 }
            );

            db.SaveChanges();

            return new TaskService(
                new TaskRepository(db),
                new TeamRepository(db),
                new UserRepository(db),
                new FakeNotificationService());
        }

        [Fact]
        public async Task GetTasks_AsAdmin_ReturnsAllTasks()
        {
            var service = CreateService(out _);

            var tasks = await service.GetTasksAsync(currentUserId: 1, currentRole: "Admin", new TaskFilterDto());

            tasks.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetTasks_AsManager_ReturnsOnlyTheirTeamsTasks()
        {
            var service = CreateService(out _);

            var tasks = await service.GetTasksAsync(currentUserId: 2, currentRole: "Manager", new TaskFilterDto());

            tasks.Should().ContainSingle(t => t.Title == "Task for team 1");
        }

        [Fact]
        public async Task GetTasks_AsUser_ReturnsOnlyTasksAssignedToThem()
        {
            var service = CreateService(out _);

            var tasks = await service.GetTasksAsync(currentUserId: 5, currentRole: "User", new TaskFilterDto());

            tasks.Should().ContainSingle(t => t.Title == "Task for team 2");
        }

        [Fact]
        public async Task CreateTask_AsManagerForAnotherManagersTeam_ThrowsForbidden()
        {
            var service = CreateService(out _);
            var dto = new CreateTaskDto { Title = "Sneaky task", TeamId = 2, AssignedToId = 5 };

            var act = () => service.CreateTaskAsync(dto, currentUserId: 2, currentRole: "Manager");

            await act.Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        public async Task CreateTask_AsManagerForOwnTeam_SucceedsAndDefaultsStatusToToDo()
        {
            var service = CreateService(out _);
            var dto = new CreateTaskDto { Title = "New task", TeamId = 1, AssignedToId = 4 };

            var task = await service.CreateTaskAsync(dto, currentUserId: 2, currentRole: "Manager");

            task.Status.Should().Be("ToDo");
        }

        [Fact]
        public async Task UpdateStatus_AsUnrelatedUser_ThrowsForbidden()
        {
            var service = CreateService(out _);

            var act = () => service.UpdateStatusAsync(2, new UpdateTaskStatusDto { Status = TaskStatusEnum.Done }, currentUserId: 4, currentRole: "User");

            await act.Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        public async Task UpdateStatus_AsAssignedUser_Succeeds()
        {
            var service = CreateService(out var db);

            await service.UpdateStatusAsync(1, new UpdateTaskStatusDto { Status = TaskStatusEnum.InProgress }, currentUserId: 4, currentRole: "User");

            db.TaskItems.Find(1)!.Status.Should().Be(TaskStatusEnum.InProgress);
        }
    }
}
