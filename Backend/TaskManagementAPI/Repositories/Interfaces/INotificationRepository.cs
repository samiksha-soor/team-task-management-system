using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        IQueryable<Notification> QueryByUser(int userId);
        Task<Notification?> GetByIdForUserAsync(int id, int userId);
    }
}
