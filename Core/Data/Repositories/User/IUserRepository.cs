using Core.Models;

namespace Core.Data.Repositories.User
{
    public interface IUserRepository
    {
        Task<AppUser?> FindByNameOrEmailAsync(string userNameOrEmail);
        Task<bool> IsUserNameTakenAsync(string userName);
        Task<bool> IsEmailTakenAsync(string email);
    }
}
