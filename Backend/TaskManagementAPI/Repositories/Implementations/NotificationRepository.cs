using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories.Interfaces;

namespace TaskManagementAPI.Repositories.Implementations
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<Notification> QueryByUser(int userId) =>
            _dbSet.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt);

        public async Task<Notification?> GetByIdForUserAsync(int id, int userId) =>
            await _dbSet.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
    }
}
