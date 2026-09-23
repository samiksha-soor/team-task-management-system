using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories.Interfaces;

namespace TaskManagementAPI.Repositories.Implementations
{
    public class TaskRepository : Repository<TaskItem>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<TaskItem> QueryWithDetails() =>
            _dbSet.Include(t => t.Team).Include(t => t.AssignedTo).Include(t => t.CreatedBy);

        public async Task<TaskItem?> GetByIdWithDetailsAsync(int id) =>
            await QueryWithDetails().FirstOrDefaultAsync(t => t.Id == id);

        public async Task<TaskItem?> GetByIdWithTeamAsync(int id) =>
            await _dbSet.Include(t => t.Team).FirstOrDefaultAsync(t => t.Id == id);
    }
}
