using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories.Interfaces;
using TaskManagementAPI.Services.Interfaces;

namespace TaskManagementAPI.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly ITaskRepository _taskRepository;

        public DashboardService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(int currentUserId, string currentRole)
        {
            var query = _taskRepository.QueryWithDetails();

            if (currentRole == "User")
                query = query.Where(t => t.AssignedToId == currentUserId);
            else if (currentRole == "Manager")
                query = query.Where(t => t.Team!.ManagerId == currentUserId);

            var tasks = await query.ToListAsync();

            return new DashboardSummaryDto
            {
                TotalTasks = tasks.Count,
                ToDoCount = tasks.Count(t => t.Status == TaskStatusEnum.ToDo),
                InProgressCount = tasks.Count(t => t.Status == TaskStatusEnum.InProgress),
                DoneCount = tasks.Count(t => t.Status == TaskStatusEnum.Done),
                OverdueCount = tasks.Count(t => t.Deadline.HasValue && t.Deadline < DateTime.UtcNow && t.Status != TaskStatusEnum.Done),
                Tasks = tasks.OrderByDescending(t => t.CreatedAt).Select(TaskMapper.ToDto).ToList()
            };
        }
    }
}
