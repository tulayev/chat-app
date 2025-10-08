using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.DTOs.Auth
{
    public record RegisterRequestDto(string Username, string Email, string Password, IFormFile? Avatar);
}
