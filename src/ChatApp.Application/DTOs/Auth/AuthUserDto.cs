namespace ChatApp.Application.DTOs.Auth
{
    public record AuthUserDto(int Id, string Token, string Username, string Email, string? AvatarUrl);
}
