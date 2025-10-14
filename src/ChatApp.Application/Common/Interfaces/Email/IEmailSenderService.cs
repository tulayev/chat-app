namespace ChatApp.Application.Common.Interfaces.Email
{
    public interface IEmailSenderService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
