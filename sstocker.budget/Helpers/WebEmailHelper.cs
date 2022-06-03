using sstocker.core.Helpers;
using sstocker.core.Repositories;
using System.IO;
using System.Linq;

namespace sstocker.budget.Helpers
{
    public class WebEmailHelper
    {
        public void SendWeeklyEmail(long accountId)
        {
            var setting = SettingsHelper.GetEmailSetting(SettingsHelper.GetEmailSettings(accountId));

            var htmlFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "WeeklyEmail.html", SearchOption.AllDirectories);
            var html = File.ReadAllText(htmlFiles.Single());

            if (setting != null && setting.SendWeeklyEmail)
            {
                SendEmail(accountId, setting.Email, "Weekly Email", html);
            }
        }

        public void SendMonthlyEmail(long accountId)
        {
            var setting = SettingsHelper.GetEmailSetting(SettingsHelper.GetEmailSettings(accountId));

            var htmlFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "MonthlyEmail.html", SearchOption.AllDirectories);
            var html = File.ReadAllText(htmlFiles.Single());

            if (setting != null && setting.SendMonthlyEmail)
            {
                SendEmail(accountId, setting.Email, "Monthly Email", html);
            }
        }

        public void SendReminderEmail(long accountId)
        {
            var setting = SettingsHelper.GetEmailSetting(SettingsHelper.GetEmailSettings(accountId));

            var htmlFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "ReminderEmail.html", SearchOption.AllDirectories);
            var html = File.ReadAllText(htmlFiles.Single());

            if (setting != null && setting.SendReminderEmail)
            {
                SendEmail(accountId, setting.Email, "Reminder", html);
            }
        }

        private void SendEmail(long accountId, string email, string subject, string bodyHtml)
        {
            var account = AccountRepository.GetAccount(accountId);
            EmailHelper.SendEmail(email, account.Name, subject, bodyHtml);
        }
    }
}