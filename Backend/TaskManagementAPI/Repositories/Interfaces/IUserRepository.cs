using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
