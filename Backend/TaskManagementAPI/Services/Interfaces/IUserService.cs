using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserSummaryDto>> GetUsersAsync();
        Task<UserSummaryDto> UpdateRoleAsync(int id, UserRole role);
    }
}
