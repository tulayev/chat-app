using Microsoft.AspNetCore.Http;

namespace Core.Models.DTOs.Auth
{
    public record RegisterRequestDto(string Username, string Email, string Password, IFormFile? Avatar);
}
