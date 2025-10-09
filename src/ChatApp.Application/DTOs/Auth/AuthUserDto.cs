namespace ChatApp.Application.DTOs.Auth
{
    public record AuthUserDto(string Token, string Username, string Email, string? AvatarUrl);
}
