using ChatApp.Application.DTOs.User;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestHelpers;
using Mapster;

namespace ChatApp.Tests.Mappings
{
    public class UserMappingTests
    {
        [Fact]
        public void Adapt_AppUserToUserDto_MapsUserNameToUsername()
        {
            var source = new AppUser { Id = 1, UserName = "alice", Email = "alice@example.com" };

            var dto = source.Adapt<UserDto>(MapsterTestConfig.Instance);

            Assert.Equal("alice", dto.Username);
        }

        [Fact]
        public void Adapt_AppUserToUserDto_MapsRemainingFieldsDirectly()
        {
            var source = new AppUser
            {
                Id = 7,
                UserName = "bob",
                Email = "bob@example.com",
                AvatarUrl = "http://img/bob.png",
                EmailConfirmed = true
            };

            var dto = source.Adapt<UserDto>(MapsterTestConfig.Instance);

            Assert.Equal(7, dto.Id);
            Assert.Equal("bob@example.com", dto.Email);
            Assert.Equal("http://img/bob.png", dto.AvatarUrl);
            Assert.True(dto.EmailConfirmed);
        }
    }
}
