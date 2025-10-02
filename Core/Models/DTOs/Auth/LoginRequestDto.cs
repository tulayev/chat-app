namespace Core.Models.DTOs.Auth
{
    public record LoginRequestDto(string UserNameOrEmail, string Password);
}
