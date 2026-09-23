using TaskManagementAPI.DTOs;

namespace TaskManagementAPI.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync(int currentUserId, string currentRole);
    }
}
