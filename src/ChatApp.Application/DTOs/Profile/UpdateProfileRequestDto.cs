using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.DTOs.Profile
{
    public record UpdateProfileRequestDto(string Username, IFormFile? Avatar);
}
