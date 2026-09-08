namespace ChatApp.Infrastructure.Resilience
{
    public static class ResiliencePipelineKeys
    {
        public const string RedisVerificationCode = "redis-verification-code";
        public const string SmtpEmail = "smtp-email";
        public const string UnreadMessagesEmailThrottle = "unread-messages-email-throttle";
    }
}
