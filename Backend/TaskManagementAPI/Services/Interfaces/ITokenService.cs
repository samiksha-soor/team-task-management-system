using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services.Interfaces
{
    public interface ITokenService
    {
        (string token, DateTime expiresAt) GenerateToken(User user);
    }
}
