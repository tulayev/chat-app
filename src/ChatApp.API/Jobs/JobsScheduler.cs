using Hangfire;

namespace ChatApp.API.Jobs
{
    public static class JobsScheduler
    {
        public static void ScheduleEmailNotificationForUnreadMessagesJob(IRecurringJobManager recurringJobManager)
        {
            recurringJobManager.AddOrUpdate<EmailNotificationForUnreadMessagesJob>(
                nameof(EmailNotificationForUnreadMessagesJob),
                job => job.ExecuteAsync(JobCancellationToken.Null),
                Cron.Daily(9));
        }
    }
}
