using FluentAssertions;
using Microsoft.Extensions.Configuration;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services.Implementations;
using Xunit;

namespace TaskManagementAPI.Tests.Services
{
    public class TokenServiceTests
    {
        private static IConfiguration BuildConfig() =>
            new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "unit-test-only-secret-key-must-be-long-enough-32chars",
                ["JwtSettings:Issuer"] = "TestIssuer",
                ["JwtSettings:Audience"] = "TestAudience",
                ["JwtSettings:ExpiresInMinutes"] = "30"
            }).Build();

        [Fact]
        public void GenerateToken_ReturnsNonEmptyTokenWithFutureExpiry()
        {
            var service = new TokenService(BuildConfig());
            var user = new User { Id = 1, FullName = "Test User", Email = "test@example.com", Role = UserRole.Manager };

            var (token, expiresAt) = service.GenerateToken(user);

            token.Should().NotBeNullOrWhiteSpace();
            token.Split('.').Should().HaveCount(3);
            expiresAt.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public void GenerateToken_EmbedsCorrectRoleClaim()
        {
            var service = new TokenService(BuildConfig());
            var user = new User { Id = 2, FullName = "Admin User", Email = "admin@example.com", Role = UserRole.Admin };

            var (token, _) = service.GenerateToken(user);

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            jwt.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Admin");
        }
    }
}
