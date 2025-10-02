using Core.Models;

namespace Core.Services.JwtToken
{
    public interface IJwtTokenService
    {
        string CreateToken(AppUser user);
    }
}
