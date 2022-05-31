using sstocker.core.Helpers;
using sstocker.core.Repositories;

namespace sstocker.budget.Helpers
{
    public static class WebEmailHelper
    {
        public static void SendWeeklyEmail(long accountId)
        {
            var setting = SettingsHelper.GetEmailSetting(SettingsHelper.GetEmailSettings(accountId));

            if (setting != null && setting.SendWeeklyEmail)
            {
                SendEmail(accountId, setting.Email, "Test Subject", "<b>Test Mail</b><br>using <b>HTML</b>.");
            }
        }

        public static void SendMonthlyEmail(long accountId)
        {
            var setting = SettingsHelper.GetEmailSetting(SettingsHelper.GetEmailSettings(accountId));

            if (setting != null && setting.SendMonthlyEmail)
            {
                SendEmail(accountId, setting.Email, "Test Subject", "<b>Test Mail</b><br>using <b>HTML</b>.");
            }
        }

        public static void SendReminderEmail(long accountId)
        {
            var setting = SettingsHelper.GetEmailSetting(SettingsHelper.GetEmailSettings(accountId));

            if (setting != null && setting.SendReminderEmail)
            {
                SendEmail(accountId, setting.Email, "Test Subject", "<b>Test Mail</b><br>using <b>HTML</b>.");
            }
        }

        private static void SendEmail(long accountId, string email, string subject, string bodyHtml)
        {
            var account = AccountRepository.GetAccount(accountId);
            EmailHelper.SendEmail(email, account.Name, subject, bodyHtml);
        }
    }
}