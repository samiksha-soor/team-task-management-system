using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Exceptions;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories.Interfaces;
using TaskManagementAPI.Services.Interfaces;

namespace TaskManagementAPI.Services.Implementations
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;

        public TeamService(ITeamRepository teamRepository, IUserRepository userRepository)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<TeamResponseDto>> GetTeamsAsync(int currentUserId, string currentRole)
        {
            var query = _teamRepository.QueryWithDetails();

            if (currentRole == "Manager")
                query = query.Where(t => t.ManagerId == currentUserId);
            else if (currentRole == "User")
                query = query.Where(t => t.Members.Any(m => m.Id == currentUserId));

            var teams = await query.ToListAsync();
            return teams.Select(MapToDto);
        }

        public async Task<TeamResponseDto> GetTeamAsync(int id, int currentUserId, string currentRole)
        {
            var team = await _teamRepository.GetByIdWithDetailsAsync(id);
            if (team == null) throw new NotFoundException("Team not found.");

            if (currentRole == "Manager" && team.ManagerId != currentUserId)
                throw new ForbiddenAccessException("You do not manage this team.");
            if (currentRole == "User" && !team.Members.Any(m => m.Id == currentUserId))
                throw new ForbiddenAccessException("You are not a member of this team.");

            return MapToDto(team);
        }

        public async Task<(int Id, string Name)> CreateTeamAsync(CreateTeamDto dto)
        {
            var manager = await _userRepository.GetByIdAsync(dto.ManagerId);
            if (manager == null) throw new BadRequestException("Manager not found.");
            if (manager.Role != UserRole.Manager && manager.Role != UserRole.Admin)
                throw new BadRequestException("Assigned manager must have Manager or Admin role.");

            var team = new TeamEntity
            {
                Name = dto.Name,
                Description = dto.Description,
                ManagerId = dto.ManagerId
            };

            await _teamRepository.AddAsync(team);
            await _teamRepository.SaveChangesAsync();

            return (team.Id, team.Name);
        }

        public async Task<string> AddMemberAsync(int teamId, AssignMemberDto dto, int currentUserId, string currentRole)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null) throw new NotFoundException("Team not found.");

            if (currentRole == "Manager" && team.ManagerId != currentUserId)
                throw new ForbiddenAccessException("You do not manage this team.");

            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null) throw new BadRequestException("User not found.");

            user.TeamId = team.Id;
            await _userRepository.SaveChangesAsync();

            return $"{user.FullName} added to team {team.Name}.";
        }

        public async Task RemoveMemberAsync(int teamId, int userId, int currentUserId, string currentRole)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null) throw new NotFoundException("Team not found.");

            if (currentRole == "Manager" && team.ManagerId != currentUserId)
                throw new ForbiddenAccessException("You do not manage this team.");

            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Id == userId && u.TeamId == teamId);
            if (user == null) throw new NotFoundException("Member not found in this team.");

            user.TeamId = null;
            await _userRepository.SaveChangesAsync();
        }

        private static TeamResponseDto MapToDto(TeamEntity t) => new()
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            ManagerId = t.ManagerId,
            ManagerName = t.Manager!.FullName,
            Members = t.Members.Select(m => new TeamMemberDto
            {
                Id = m.Id,
                FullName = m.FullName,
                Email = m.Email,
                Role = m.Role.ToString()
            }).ToList()
        };
    }
}
