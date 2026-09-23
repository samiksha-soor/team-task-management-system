using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories.Interfaces
{
    public interface ITaskCommentRepository : IRepository<TaskComment>
    {
        IQueryable<TaskComment> QueryByTask(int taskId);
    }
}
