using ChatApp.Application.Common.Interfaces.Email;
using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Domain.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Jobs
{
    public class EmailNotificationForUnreadMessagesJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ILogger<EmailNotificationForUnreadMessagesJob> _logger;

        public EmailNotificationForUnreadMessagesJob(IUnitOfWork unitOfWork,
            IEmailSenderService emailSenderService,
            ILogger<EmailNotificationForUnreadMessagesJob> logger)
        {
            _unitOfWork = unitOfWork;
            _emailSenderService = emailSenderService;
            _logger = logger;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 60)]
        public async Task ExecuteAsync(IJobCancellationToken cancellationToken)
        {
            var ct = cancellationToken.ShutdownToken;

            var unreadRows = await _unitOfWork.GetQueryable<Message>()
                .Where(m => m.ReadAt == null)
                .Select(m => new
                {
                    RecipientId = m.Chat.User1Id == m.SenderId ? m.Chat.User2Id : m.Chat.User1Id,
                    m.ChatId
                })
                .ToListAsync(ct);

            if (unreadRows.Count == 0)
            {
                _logger.LogInformation("No unread messages found; skipping notification job.");
                return;
            }

            var unreadByRecipient = unreadRows
                .GroupBy(x => x.RecipientId)
                .Select(g => new
                {
                    RecipientId = g.Key,
                    UnreadCount = g.Count(),
                    ChatCount = g.Select(x => x.ChatId).Distinct().Count()
                })
                .ToList();

            var recipientIds = unreadByRecipient.Select(x => x.RecipientId).ToHashSet();
            var recipients = await _unitOfWork.GetQueryable<AppUser>()
                .Where(u => recipientIds.Contains(u.Id) && u.EmailConfirmed && u.Email != null)
                .Select(u => new { u.Id, u.Email, u.UserName })
                .ToListAsync(ct);

            var sent = 0;

            foreach (var recipient in recipients)
            {
                var stats = unreadByRecipient.First(x => x.RecipientId == recipient.Id);
                var subject = "You have unread messages";
                var body = $"""
                    <h2>Hi {recipient.UserName},</h2>
                    <p>You have <strong>{stats.UnreadCount}</strong> unread message{(stats.UnreadCount == 1 ? "" : "s")}
                    across {stats.ChatCount} chat{(stats.ChatCount == 1 ? "" : "s")}.</p>
                    <p>Log in to ChatApp to read {(stats.UnreadCount == 1 ? "it" : "them")}.</p>
                    """;

                try
                {
                    await _emailSenderService.SendAsync(recipient.Email!, subject, body);
                    sent++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send unread-messages notification to user {UserId}", recipient.Id);
                }
            }

            _logger.LogInformation("Sent unread-messages notification to {Sent} of {Total} recipients.", sent, recipients.Count);
        }
    }
}
