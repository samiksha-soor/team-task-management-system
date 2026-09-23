using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories.Interfaces;

namespace TaskManagementAPI.Repositories.Implementations
{
    public class TaskCommentRepository : Repository<TaskComment>, ITaskCommentRepository
    {
        public TaskCommentRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<TaskComment> QueryByTask(int taskId) =>
            _dbSet.Include(c => c.User).Where(c => c.TaskItemId == taskId).OrderBy(c => c.CreatedAt);
    }
}
