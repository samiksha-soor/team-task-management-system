using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Exceptions;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories.Interfaces;
using TaskManagementAPI.Services.Interfaces;

namespace TaskManagementAPI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserSummaryDto>> GetUsersAsync()
        {
            return await _userRepository.Query()
                .Select(u => new UserSummaryDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    TeamId = u.TeamId
                })
                .ToListAsync();
        }

        public async Task<UserSummaryDto> UpdateRoleAsync(int id, UserRole role)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new NotFoundException("User not found.");

            user.Role = role;
            await _userRepository.SaveChangesAsync();

            return new UserSummaryDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                TeamId = user.TeamId
            };
        }
    }
}
