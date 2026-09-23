using TaskManagementAPI.DTOs;

namespace TaskManagementAPI.Services.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamResponseDto>> GetTeamsAsync(int currentUserId, string currentRole);
        Task<TeamResponseDto> GetTeamAsync(int id, int currentUserId, string currentRole);
        Task<(int Id, string Name)> CreateTeamAsync(CreateTeamDto dto);
        Task<string> AddMemberAsync(int teamId, AssignMemberDto dto, int currentUserId, string currentRole);
        Task RemoveMemberAsync(int teamId, int userId, int currentUserId, string currentRole);
    }
}
