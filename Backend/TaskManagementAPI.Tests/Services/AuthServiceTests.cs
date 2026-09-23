using FluentAssertions;
using Microsoft.Extensions.Configuration;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Services.Implementations;
using TaskManagementAPI.Services.Interfaces;
using TaskManagementAPI.Tests.Helpers;
using Xunit;

namespace TaskManagementAPI.Tests.Services
{
    public class AuthServiceTests
    {
        private static ITokenService CreateTokenService()
        {
            var settings = new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "unit-test-only-secret-key-must-be-long-enough-32chars",
                ["JwtSettings:Issuer"] = "TestIssuer",
                ["JwtSettings:Audience"] = "TestAudience",
                ["JwtSettings:ExpiresInMinutes"] = "60"
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            return new TokenService(config);
        }

        [Fact]
        public async Task RegisterAsync_WithNewEmail_CreatesUserAndReturnsToken()
        {
            var db = TestDbContextFactory.Create();
            var authService = new AuthService(db, CreateTokenService());

            var dto = new RegisterDto
            {
                FullName = "Jane Doe",
                Email = "jane@example.com",
                Password = "Passw0rd!"
            };

            var result = await authService.RegisterAsync(dto);

            result.Token.Should().NotBeNullOrEmpty();
            result.Email.Should().Be("jane@example.com");
            result.Role.Should().Be("User");

            var savedUser = db.Users.Single();
            savedUser.PasswordHash.Should().NotBe("Passw0rd!");
        }

        [Fact]
        public async Task RegisterAsync_WithDuplicateEmail_ThrowsInvalidOperationException()
        {
            var db = TestDbContextFactory.Create();
            var authService = new AuthService(db, CreateTokenService());
            var dto = new RegisterDto { FullName = "Jane", Email = "dup@example.com", Password = "Passw0rd!" };

            await authService.RegisterAsync(dto);

            Func<Task> act = async () => await authService.RegisterAsync(dto);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task LoginAsync_WithCorrectPassword_ReturnsToken()
        {
            var db = TestDbContextFactory.Create();
            var authService = new AuthService(db, CreateTokenService());
            await authService.RegisterAsync(new RegisterDto { FullName = "Jane", Email = "jane2@example.com", Password = "Passw0rd!" });

            var result = await authService.LoginAsync(new LoginDto { Email = "jane2@example.com", Password = "Passw0rd!" });

            result.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ThrowsUnauthorizedAccessException()
        {
            var db = TestDbContextFactory.Create();
            var authService = new AuthService(db, CreateTokenService());
            await authService.RegisterAsync(new RegisterDto { FullName = "Jane", Email = "jane3@example.com", Password = "Passw0rd!" });

            Func<Task> act = async () => await authService.LoginAsync(
                new LoginDto { Email = "jane3@example.com", Password = "WrongPassword" });

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task LoginAsync_WithUnknownEmail_ThrowsUnauthorizedAccessException()
        {
            var db = TestDbContextFactory.Create();
            var authService = new AuthService(db, CreateTokenService());

            Func<Task> act = async () => await authService.LoginAsync(
                new LoginDto { Email = "nobody@example.com", Password = "whatever" });

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
