using ChatApp.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ChatApp.Tests.TestHelpers
{
    public static class IdentityMockFactory
    {
        public static Mock<UserManager<AppUser>> CreateUserManagerMock(IQueryable<AppUser>? users = null)
        {
            var store = new Mock<IUserStore<AppUser>>();
            var mgr = new Mock<UserManager<AppUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            if (users is not null)
            {
                mgr.Setup(x => x.Users).Returns(users);
            }

            return mgr;
        }

        public static Mock<SignInManager<AppUser>> CreateSignInManagerMock(Mock<UserManager<AppUser>> userManagerMock)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<AppUser>>();

            var signInManager = new Mock<SignInManager<AppUser>>(
                userManagerMock.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null!, null!, null!, null!);

            return signInManager;
        }
    }
}
