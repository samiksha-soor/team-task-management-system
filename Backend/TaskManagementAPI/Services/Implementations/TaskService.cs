using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Exceptions;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories.Interfaces;
using TaskManagementAPI.Services.Interfaces;

namespace TaskManagementAPI.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public TaskService(
            ITaskRepository taskRepository,
            ITeamRepository teamRepository,
            IUserRepository userRepository,
            INotificationService notificationService)
        {
            _taskRepository = taskRepository;
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<TaskResponseDto>> GetTasksAsync(int currentUserId, string currentRole, TaskFilterDto filter)
        {
            var query = _taskRepository.QueryWithDetails();

            if (currentRole == "User")
                query = query.Where(t => t.AssignedToId == currentUserId);
            else if (currentRole == "Manager")
                query = query.Where(t => t.Team!.ManagerId == currentUserId);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);
            if (filter.Priority.HasValue)
                query = query.Where(t => t.Priority == filter.Priority.Value);
            if (filter.DeadlineFrom.HasValue)
                query = query.Where(t => t.Deadline >= filter.DeadlineFrom.Value);
            if (filter.DeadlineTo.HasValue)
                query = query.Where(t => t.Deadline <= filter.DeadlineTo.Value);

            var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
            return tasks.Select(TaskMapper.ToDto);
        }

        public async Task<TaskResponseDto> GetTaskAsync(int id, int currentUserId, string currentRole)
        {
            var task = await _taskRepository.GetByIdWithDetailsAsync(id);
            if (task == null) throw new NotFoundException("Task not found.");
            EnsureCanAccessTask(task, currentUserId, currentRole);

            return TaskMapper.ToDto(task);
        }

        public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, int currentUserId, string currentRole)
        {
            var team = await _teamRepository.GetByIdAsync(dto.TeamId);
            if (team == null) throw new BadRequestException("Team not found.");

            if (currentRole == "Manager" && team.ManagerId != currentUserId)
                throw new ForbiddenAccessException("You do not manage this team.");

            var assignee = await _userRepository.GetByIdAsync(dto.AssignedToId);
            if (assignee == null) throw new BadRequestException("Assignee not found.");
            if (assignee.TeamId != dto.TeamId)
                throw new BadRequestException("Assignee is not a member of the specified team.");

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Deadline = dto.Deadline,
                TeamId = dto.TeamId,
                AssignedToId = dto.AssignedToId,
                CreatedById = currentUserId,
                Status = TaskStatusEnum.ToDo
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            await _notificationService.NotifyTaskAssignedAsync(task);

            var created = await _taskRepository.GetByIdWithDetailsAsync(task.Id);
            return TaskMapper.ToDto(created!);
        }

        public async Task UpdateTaskAsync(int id, UpdateTaskDto dto, int currentUserId, string currentRole)
        {
            var task = await _taskRepository.GetByIdWithTeamAsync(id);
            if (task == null) throw new NotFoundException("Task not found.");

            if (currentRole == "Manager" && task.Team!.ManagerId != currentUserId)
                throw new ForbiddenAccessException("You do not manage this team.");

            if (dto.Title != null) task.Title = dto.Title;
            if (dto.Description != null) task.Description = dto.Description;
            if (dto.Priority.HasValue) task.Priority = dto.Priority.Value;
            if (dto.Deadline.HasValue) task.Deadline = dto.Deadline;
            if (dto.AssignedToId.HasValue)
            {
                var assignee = await _userRepository.GetByIdAsync(dto.AssignedToId.Value);
                if (assignee == null) throw new BadRequestException("Assignee not found.");
                task.AssignedToId = dto.AssignedToId.Value;
            }
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, UpdateTaskStatusDto dto, int currentUserId, string currentRole)
        {
            var task = await _taskRepository.GetByIdWithTeamAsync(id);
            if (task == null) throw new NotFoundException("Task not found.");

            var isAssignee = task.AssignedToId == currentUserId;
            var isOwningManager = currentRole == "Manager" && task.Team!.ManagerId == currentUserId;
            var isAdmin = currentRole == "Admin";

            if (!isAssignee && !isOwningManager && !isAdmin)
                throw new ForbiddenAccessException("You cannot update the status of this task.");

            task.Status = dto.Status;
            task.UpdatedAt = DateTime.UtcNow;
            await _taskRepository.SaveChangesAsync();

            await _notificationService.NotifyTaskStatusUpdatedAsync(task);
        }

        public async Task DeleteTaskAsync(int id, int currentUserId, string currentRole)
        {
            var task = await _taskRepository.GetByIdWithTeamAsync(id);
            if (task == null) throw new NotFoundException("Task not found.");

            if (currentRole == "Manager" && task.Team!.ManagerId != currentUserId)
                throw new ForbiddenAccessException("You do not manage this team.");

            _taskRepository.Remove(task);
            await _taskRepository.SaveChangesAsync();
        }

        private static void EnsureCanAccessTask(TaskItem task, int currentUserId, string currentRole)
        {
            if (currentRole == "Admin") return;
            if (currentRole == "Manager")
            {
                if (task.Team!.ManagerId == currentUserId) return;
                throw new ForbiddenAccessException("You do not manage this team.");
            }
            if (task.AssignedToId == currentUserId) return;
            throw new ForbiddenAccessException("You cannot access this task.");
        }
    }
}
