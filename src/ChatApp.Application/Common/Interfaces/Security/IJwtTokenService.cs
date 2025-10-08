using ChatApp.Domain.Models;

namespace ChatApp.Application.Common.Interfaces.Security
{
    public interface IJwtTokenService
    {
        string CreateToken(AppUser user);
    }
}
