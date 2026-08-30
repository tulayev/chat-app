using ChatApp.Application.Common.Extensions;
using ChatApp.Domain.Models;

namespace ChatApp.Tests.Extensions
{
    public class UserQueryableExtensionsTests
    {
        [Fact]
        public void WhereEmailConfirmed_ReturnsOnlyUsersWithConfirmedEmail()
        {
            var users = new List<AppUser>
            {
                new() { Id = 1, UserName = "confirmed", EmailConfirmed = true },
                new() { Id = 2, UserName = "unconfirmed", EmailConfirmed = false }
            }.AsQueryable();

            var result = users.WhereEmailConfirmed().ToList();

            var user = Assert.Single(result);
            Assert.Equal(1, user.Id);
        }
    }
}
