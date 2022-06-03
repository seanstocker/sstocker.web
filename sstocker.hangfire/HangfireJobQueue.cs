using Hangfire;
using sstocker.budget.Helpers;
using sstocker.core.Repositories;
using System;

namespace sstocker.hangfire
{
    public static class HangfireJobQueue
    {
        private const string RecurringJobIdFormat = "{0}-{1}";

        public const string WeeklyEmailId = "weekly";
        public const string MonthlyEmailId = "monthly";
        public const string ReminderEmailId = "reminder";

        private const int DefaultEmailSendHour = 18;
        private const int DefaultMonthlyEmailSendDay = 1;
        private const DayOfWeek DefaultWeeklyEmailSendDayOfWeek = DayOfWeek.Saturday;

        public static void SetupWebEmailQueue()
        {
            var accountIds = AccountRepository.GetAllAccounts();

            foreach (var accountId in accountIds)
            {
                RefreshAccountWebEmailQueue(accountId);
            }
        }

        public static void RefreshAccountWebEmailQueue(long accountId)
        {
            var emailSettings = SettingsHelper.GetEmailSetting(SettingsHelper.GetEmailSettings(accountId));

            if (emailSettings == null || string.IsNullOrWhiteSpace(emailSettings.Email))
            {
                RemoveWeeklyEmail(accountId);
                RemoveMonthlyEmail(accountId);
                RemoveReminderEmail(accountId);
            }
            else
            {
                if (emailSettings.SendWeeklyEmail)
                {
                    StartWeeklyEmail(accountId);
                }
                else
                {
                    RemoveWeeklyEmail(accountId);
                }

                if (emailSettings.SendMonthlyEmail)
                {
                    StartMonthlyEmail(accountId);
                }
                else
                {
                    RemoveMonthlyEmail(accountId);
                }

                if (emailSettings.SendReminderEmail)
                {
                    StartReminderEmail(accountId);
                }
                else
                {
                    RemoveReminderEmail(accountId);
                }
            }
        }

        public static void StartWeeklyEmail(long accountId, DayOfWeek dayOfWeek = DefaultWeeklyEmailSendDayOfWeek, int hour = DefaultEmailSendHour)
        {
            RecurringJob.AddOrUpdate<WebEmailHelper>(string.Format(RecurringJobIdFormat, WeeklyEmailId, accountId), x => x.SendWeeklyEmail(accountId), Cron.Weekly(dayOfWeek, hour), queue: WeeklyEmailId);
        }

        public static void RemoveWeeklyEmail(long accountId)
        {
            RemoveRecurringEmail(WeeklyEmailId, accountId);
        }

        public static void StartMonthlyEmail(long accountId, int day = DefaultMonthlyEmailSendDay, int hour = DefaultEmailSendHour)
        {
            RecurringJob.AddOrUpdate<WebEmailHelper>(string.Format(RecurringJobIdFormat, MonthlyEmailId, accountId), x => x.SendMonthlyEmail(accountId), Cron.Monthly(day, hour), queue: MonthlyEmailId);
        }

        public static void RemoveMonthlyEmail(long accountId)
        {
            RemoveRecurringEmail(MonthlyEmailId, accountId);
        }

        public static void StartReminderEmail(long accountId, int hour = DefaultEmailSendHour)
        {
            RecurringJob.AddOrUpdate<WebEmailHelper>(string.Format(RecurringJobIdFormat, ReminderEmailId, accountId), x => x.SendReminderEmail(accountId), Cron.Daily(hour), queue: ReminderEmailId);
        }

        public static void RemoveReminderEmail(long accountId)
        {
            RemoveRecurringEmail(ReminderEmailId, accountId);
        }

        private static void RemoveRecurringEmail(string id, long accountId)
        {
            RecurringJob.RemoveIfExists(string.Format(RecurringJobIdFormat, id, accountId));
        }

        public static void EnqueueJob()
        {
            BackgroundJob.Enqueue(() => Console.WriteLine("Hangfire started!"));
        }
    }
}