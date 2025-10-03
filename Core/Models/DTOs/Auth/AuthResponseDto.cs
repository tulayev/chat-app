namespace Core.Models.DTOs.Auth
{
    public record AuthResponseDto(string Token, string Username, string Email, string? AvatarUrl);
}
