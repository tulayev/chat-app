using Microsoft.AspNetCore.Http;

namespace Core.Models.DTOs.Auth
{
    public record RegisterWithAvatarRequestDto(string UserName, string Email, string Password, IFormFile? Avatar);
}
