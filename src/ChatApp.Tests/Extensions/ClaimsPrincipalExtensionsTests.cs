using System.Security.Claims;
using ChatApp.Application.Common.Extensions;

namespace ChatApp.Tests.Extensions
{
    public class ClaimsPrincipalExtensionsTests
    {
        private static ClaimsPrincipal BuildPrincipal(params Claim[] claims)
        {
            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        [Fact]
        public void GetUserId_ClaimPresent_ReturnsParsedInt()
        {
            var principal = BuildPrincipal(new Claim(ClaimTypes.NameIdentifier, "42"));

            var userId = principal.GetUserId();

            Assert.Equal(42, userId);
        }

        [Fact]
        public void GetUserId_ClaimMissing_ThrowsArgumentNullException()
        {
            var principal = BuildPrincipal();

            Assert.Throws<ArgumentNullException>(() => principal.GetUserId());
        }

        [Fact]
        public void GetUsername_ClaimPresent_ReturnsValue()
        {
            var principal = BuildPrincipal(new Claim(ClaimTypes.Name, "alice"));

            Assert.Equal("alice", principal.GetUsername());
        }

        [Fact]
        public void GetUsername_ClaimMissing_ReturnsNull()
        {
            var principal = BuildPrincipal();

            Assert.Null(principal.GetUsername());
        }

        [Fact]
        public void GetEmail_ClaimPresent_ReturnsValue()
        {
            var principal = BuildPrincipal(new Claim(ClaimTypes.Email, "alice@example.com"));

            Assert.Equal("alice@example.com", principal.GetEmail());
        }

        [Fact]
        public void GetEmail_ClaimMissing_ReturnsNull()
        {
            var principal = BuildPrincipal();

            Assert.Null(principal.GetEmail());
        }
    }
}
