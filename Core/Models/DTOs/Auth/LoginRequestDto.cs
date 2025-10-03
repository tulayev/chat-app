namespace Core.Models.DTOs.Auth
{
    public record LoginRequestDto(string UsernameOrEmail, string Password);
}
