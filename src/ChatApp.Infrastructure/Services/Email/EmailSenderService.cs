using ChatApp.Application.Common.Interfaces.Email;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace ChatApp.Infrastructure.Services.Email
{
    public class SmtpSettings
    {
        public string Host { get; set; } = default!;
        public int Port { get; set; }
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string From { get; set; } = default!;
    }

    public class EmailSenderService : IEmailSenderService
    {
        private readonly SmtpSettings _settings;

        public EmailSenderService(IConfiguration config)
        {
            _settings = config.GetSection("Email:Smtp").Get<SmtpSettings>()!;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            var message = new MailMessage(_settings.From, to, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
    }
}
