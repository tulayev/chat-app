namespace ChatApp.Application.DTOs.Auth
{
    public record LoginRequestDto(string UsernameOrEmail, string Password);
}
