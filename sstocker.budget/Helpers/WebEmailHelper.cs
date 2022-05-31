using sstocker.core.Helpers;
using sstocker.core.Repositories;

namespace sstocker.budget.Helpers
{
    public static class WebEmailHelper
    {
        public static void SendTestEmail(long accountId)
        {
            SendEmail(accountId, "Test Subject", "<b>Test Mail</b><br>using <b>HTML</b>.");
        }

        private static void SendEmail(long accountId, string subject, string bodyHtml)
        {
            var account = AccountRepository.GetAccount(accountId);
            var setting = SettingsHelper.GetEmailSetting(SettingsHelper.GetEmailSettings(accountId));
            if (setting != null && setting.SendEmail)
            {
                EmailHelper.SendEmail(setting.Email, account.Name, subject, bodyHtml);
            }
        }
    }
}
