using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Data.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AppUser?> FindByNameOrEmailAsync(string userNameOrEmail)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userNameOrEmail);

            if (user != null)
            {
                return user;
            }

            return await _userManager.FindByEmailAsync(userNameOrEmail);
        }

        public async Task<bool> IsUserNameTakenAsync(string userName)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == userName);
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            return await _userManager.Users.AnyAsync(u => u.Email == email);
        }
    }
}
