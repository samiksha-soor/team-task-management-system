using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories.Interfaces
{
    public interface ITaskRepository : IRepository<TaskItem>
    {
        IQueryable<TaskItem> QueryWithDetails();
        Task<TaskItem?> GetByIdWithDetailsAsync(int id);
        Task<TaskItem?> GetByIdWithTeamAsync(int id);
    }
}
