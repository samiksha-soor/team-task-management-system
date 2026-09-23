using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services.Interfaces;

namespace TaskManagementAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existing != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var (token, expiresAt) = _tokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var (token, expiresAt) = _tokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}
