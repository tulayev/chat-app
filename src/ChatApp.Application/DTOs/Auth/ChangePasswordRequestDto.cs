namespace ChatApp.Application.DTOs.Auth
{
    public record ChangePasswordRequestDto(string CurrentPassword, string NewPassword);
}
