using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ChatApp.Domain.Models;
using ChatApp.Infrastructure.Services.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace ChatApp.Tests.Infrastructure.Security
{
    public class JwtTokenServiceTests
    {
        private const string TestKey = "a-sufficiently-long-test-signing-key-for-hmacsha256";

        private static JwtTokenService BuildService(string key = TestKey)
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["JwtTokenKey"]).Returns(key);
            return new JwtTokenService(configMock.Object);
        }

        private static AppUser BuildUser() => new()
        {
            Id = 7,
            UserName = "alice",
            Email = "alice@example.com"
        };

        [Fact]
        public void CreateToken_ReturnsNonEmptyJwtString()
        {
            var service = BuildService();

            var token = service.CreateToken(BuildUser());

            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public void CreateToken_TokenContainsExpectedClaims()
        {
            var service = BuildService();
            var user = BuildUser();

            var token = service.CreateToken(user);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            Assert.Equal(user.Id.ToString(), jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.NameId).Value);
            Assert.Equal(user.UserName, jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);
            Assert.Equal(user.Email, jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        }

        [Fact]
        public void CreateToken_ExpiryIsApproximatelyEightHoursFromNow()
        {
            var service = BuildService();

            var token = service.CreateToken(BuildUser());
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var expected = DateTime.UtcNow.AddHours(8);
            Assert.True(Math.Abs((jwt.ValidTo - expected).TotalMinutes) < 1);
        }

        [Fact]
        public void CreateToken_SignedWithConfiguredKey_ValidatesSuccessfullyWithSameKey()
        {
            var service = BuildService();
            var token = service.CreateToken(BuildUser());

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestKey)),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var exception = Record.Exception(() =>
                new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _));

            Assert.Null(exception);
        }

        [Fact]
        public void CreateToken_SignedWithConfiguredKey_FailsValidationWithDifferentKey()
        {
            var service = BuildService();
            var token = service.CreateToken(BuildUser());

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("a-completely-different-signing-key-value")),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            Assert.ThrowsAny<SecurityTokenException>(() =>
                new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _));
        }
    }
}
