namespace Core.Models.DTOs.Auth
{
    public record AuthResponse(string Token, string Username, string Email, string? AvatarUrl);
}
