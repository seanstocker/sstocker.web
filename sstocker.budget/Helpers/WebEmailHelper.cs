using sstocker.budget.Repositories;
using sstocker.core.Helpers;
using sstocker.core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace sstocker.budget.Helpers
{
    public class WebEmailHelper
    {
        public void SendWeeklyEmail(long accountId)
        {
            var setting = SettingsHelper.GetEmailSetting(SettingsHelper.GetEmailSettings(accountId));

            var expenses = ExpenseRepository.GetAccountExpenses(accountId).Where(e => e.SpentDate.Date < DateTime.Today && e.SpentDate.Date >= DateTime.Today.AddDays(-7));

            var html = File.ReadAllText(GetFilePath("WeeklyEmail.html"));
            html = ReplaceTags(html, new List<(string, string)> { });
            var images = new List<(string, string)> { };

            if (setting != null && setting.SendWeeklyEmail)
            {
                SendEmail(accountId, setting.Email, "Weekly Email", html, images);
            }
        }

        public void SendMonthlyEmail(long accountId)
        {
            var setting = SettingsHelper.GetEmailSetting(SettingsHelper.GetEmailSettings(accountId));

            var html = File.ReadAllText(GetFilePath("MonthlyEmail.html"));
            html = ReplaceTags(html, new List<(string, string)> { });
            var images = new List<(string, string)> { };

            if (setting != null && setting.SendMonthlyEmail)
            {
                SendEmail(accountId, setting.Email, "Monthly Email", html, images);
            }
        }

        public void SendReminderEmail(long accountId)
        {
            var setting = SettingsHelper.GetEmailSetting(SettingsHelper.GetEmailSettings(accountId));

            var expenses = ExpenseRepository.GetAccountExpenses(accountId);

            if(expenses.Any(e=>e.SpentDate.Date == DateTime.Today))
            {
                Console.WriteLine("No need for reminder email.");
                return;
            }

            var html = File.ReadAllText(GetFilePath("ReminderEmail.html"));
            html = ReplaceTags(html, new List<(string, string)>
            {
                ("Date", DateTime.Today.ToShortDateString())
            });

            var images = new List<(string, string)>
            {
                ("banner", GetFilePath("banner.jpg"))
            };

            if (setting != null && setting.SendReminderEmail)
            {
                SendEmail(accountId, setting.Email, "Reminder", html, images);
            }
        }

        private void SendEmail(long accountId, string email, string subject, string bodyHtml, List<(string, string)> images)
        {
            var account = AccountRepository.GetAccount(accountId);
            EmailHelper.SendEmail(email, account.Name, subject, bodyHtml, images);
        }

        private string ReplaceTags(string html, IList<(string, string)> values)
        {
            foreach (var value in values)
            {
                html = html.Replace($"{{{value.Item1}}}", value.Item2);
            }

            if (Regex.IsMatch(html, @"{[[:alpha:]]+}"))
            {
                throw new Exception("Cannot create email. Missing values.");
            }

            return html;
        }

        private string GetFilePath(string name)
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), name, SearchOption.AllDirectories);
            return files.Single();
        }
    }
}