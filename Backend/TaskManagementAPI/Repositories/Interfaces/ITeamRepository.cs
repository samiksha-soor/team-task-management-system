using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories.Interfaces
{
    public interface ITeamRepository : IRepository<TeamEntity>
    {
        IQueryable<TeamEntity> QueryWithDetails();
        Task<TeamEntity?> GetByIdWithDetailsAsync(int id);
    }
}
