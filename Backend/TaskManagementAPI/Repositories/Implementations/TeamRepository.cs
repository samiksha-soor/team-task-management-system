using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories.Interfaces;

namespace TaskManagementAPI.Repositories.Implementations
{
    public class TeamRepository : Repository<TeamEntity>, ITeamRepository
    {
        public TeamRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<TeamEntity> QueryWithDetails() =>
            _dbSet.Include(t => t.Manager).Include(t => t.Members);

        public async Task<TeamEntity?> GetByIdWithDetailsAsync(int id) =>
            await QueryWithDetails().FirstOrDefaultAsync(t => t.Id == id);
    }
}
