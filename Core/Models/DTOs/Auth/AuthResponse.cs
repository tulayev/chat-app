namespace Core.Models.DTOs.Auth
{
    public record AuthResponse(string Token, string UserName, string Email, string? AvatarUrl);
}
