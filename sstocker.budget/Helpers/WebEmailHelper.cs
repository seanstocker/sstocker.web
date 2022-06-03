using sstocker.budget.Repositories;
using sstocker.core.Helpers;
using sstocker.core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace sstocker.budget.Helpers
{
    public class WebEmailHelper
    {
        public void SendWeeklyEmail(long accountId)
        {
            var setting = SettingsHelper.GetEmailSetting(SettingsHelper.GetEmailSettings(accountId));

            var endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
            var startDate = endDate.AddDays(-7).Date;

            var allExpenses = ExpenseRepository.GetAccountExpenses(accountId);
            var expenses = allExpenses.Where(e => e.SpentDate.Date <= endDate && e.SpentDate.Date >= startDate);

            if(!expenses.Any())
            {
                return;
            }

            var html = File.ReadAllText(GetFilePath("WeeklyEmail.html"));

            html = ReplaceTags(html, new List<(string, string)>
            {
                ("StartDate", startDate.ToShortDateString()),
                ("EndDate", endDate.ToShortDateString()),
                ("Total", $"{expenses.Sum(e=>e.Amount):C}"),
                ("MonthlyTotal", $"{allExpenses.Where(e=>e.SpentDate.Year == endDate.Year && e.SpentDate.Month == endDate.Month).Sum(e=>e.Amount):C}"),
                ("PercentMonday", $"{expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Monday).Sum(e=>e.Amount):C}"),
                ("PercentTuesday", $"{expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Tuesday).Sum(e=>e.Amount):C}"),
                ("PercentWednesday", $"{expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Wednesday).Sum(e=>e.Amount):C}"),
                ("PercentThursday", $"{expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Thursday).Sum(e=>e.Amount):C}"),
                ("PercentFriday", $"{expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Friday).Sum(e=>e.Amount):C}"),
                ("PercentSaturday", $"{expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Saturday).Sum(e=>e.Amount):C}"),
                ("PercentSunday", $"{expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Sunday).Sum(e=>e.Amount):C}")
            });

            var charts = new List<(string, string)>
            {
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Monday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "ff5b74"), "piemon", $"weekly-{accountId}"),
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Tuesday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "8b0e8b"), "pietue", $"weekly-{accountId}"),
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Wednesday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "d6a1ff"), "piewed", $"weekly-{accountId}"),
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Thursday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "ff5b74"), "piethu", $"weekly-{accountId}"),
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Friday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "8b0e8b"), "piefri", $"weekly-{accountId}"),
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Saturday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "d6a1ff"), "piesat", $"weekly-{accountId}"),
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Sunday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "ff5b74"), "piesun", $"weekly-{accountId}"),
                CreateChart(PieChart(expenses.GroupBy(e => e.Category).Select(g => (g.Key, (int)g.Sum(x => x.Amount)))), "piecategory", $"weekly-{accountId}")
            };

            var images = new List<(string, string)>
            {
                ("headerbackground", GetFilePath("weekly-header background.png")),
                ("banner", GetFilePath("weekly-banner.png")),
                ("bodybackground", GetFilePath("weekly-body background.png"))
            };
            images.AddRange(charts);

            if (setting != null && setting.SendWeeklyEmail)
            {
                SendEmail(accountId, setting.Email, "Weekly Email", html, images);
            }

            DeleteCharts(charts);
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
                ("banner", GetFilePath("reminder-banner.jpg"))
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

        private (string, string) CreateChart(string url, string fileName, string folderName)
        {
            if(!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            var path = $"{folderName}/{fileName}.png";

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(url, path);
            }

            return (fileName, Path.GetFullPath(path));
        }

        private void DeleteCharts(List<(string, string)> charts)
        {
            if(charts == null || !charts.Any())
            {
                return;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            foreach (var chart in charts)
            {
                File.Delete(chart.Item2);
            }
        }

        private string RadialGauge(int percentage, string color)
        {
            var url = "https://quickchart.io/chart?c={%20type:%20%27radialGauge%27,%20data:%20{%20datasets:%20[{%20data:%20[{percentage}],%20backgroundColor:%27%23{hex}%27%20}]%20}%20}";
            return url.Replace("{percentage}", percentage.ToString()).Replace("{hex}", color);
        }

        private string PieChart(IEnumerable<(string, int)> data)
        {
            var labels = string.Join("%2C", data.Select(d => $"%27{d.Item1}%27"));
            var dataSet = string.Join("%2C", data.Select(d => $"{d.Item2}"));
            var url = "https://quickchart.io/chart?c=%7B%0A%20%20type%3A%20%27pie%27%2C%0A%20%20data%3A%20%7B%0A%20%20%20%20labels%3A%20%5B{labels}%5D%2C%0A%20%20%20%20datasets%3A%20%5B%7B%0A%20%20%20%20%20%20data%3A%20%5B{dataset}%5D%0A%20%20%20%20%7D%5D%0A%20%20%7D%0A%7D%0A";
            return url.Replace("{labels}", labels).Replace("{dataset}", dataSet);
        }
    }
}