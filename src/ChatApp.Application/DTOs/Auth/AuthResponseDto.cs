namespace ChatApp.Application.DTOs.Auth
{
    public record AuthResponseDto(string Token, string Username, string Email, string? AvatarUrl);
}
