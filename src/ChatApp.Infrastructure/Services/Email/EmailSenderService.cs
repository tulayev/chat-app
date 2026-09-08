using ChatApp.Application.Common.Interfaces.Email;
using ChatApp.Infrastructure.Resilience;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
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
        private readonly ResiliencePipeline _pipeline;
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(IConfiguration config, 
            ResiliencePipelineProvider<string> pipelineProvider, 
            ILogger<EmailSenderService> logger)
        {
            _settings = config.GetSection("Email:Smtp").Get<SmtpSettings>()!;
            _pipeline = pipelineProvider.GetPipeline(ResiliencePipelineKeys.SmtpEmail);
            _logger = logger;
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

            await _pipeline.ExecuteAsync(async ct => await client.SendMailAsync(message, ct));

            _logger.LogInformation("Email sent to '{To}'", to);
        }
    }
}
