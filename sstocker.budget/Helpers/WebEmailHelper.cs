using QuickChart;
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
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Monday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "#ff5b74"), "piemon", $"weekly-{accountId}"),
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Tuesday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "#8b0e8b"), "pietue", $"weekly-{accountId}"),
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Wednesday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "#d6a1ff"), "piewed", $"weekly-{accountId}"),
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Thursday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "#ff5b74"), "piethu", $"weekly-{accountId}"),
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Friday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "#8b0e8b"), "piefri", $"weekly-{accountId}"),
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Saturday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "#d6a1ff"), "piesat", $"weekly-{accountId}"),
                CreateChart(RadialGauge((int)Math.Round(expenses.Where(e=>e.SpentDate.DayOfWeek == DayOfWeek.Sunday).Sum(e=>e.Amount)/expenses.Sum(e=>e.Amount)*100), "#ff5b74"), "piesun", $"weekly-{accountId}"),
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
                ("banner", GetFilePath("reminder-banner.png"))
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

        private (string, string) CreateChart(Chart chart, string fileName, string folderName)
        {
            if(!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            var path = $"{folderName}/{fileName}.png";

            chart.ToFile(path);

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

        private Chart RadialGauge(int percentage, string color)
        {
            Chart chart = new Chart();

            chart.Config = $@"
{{
  type: 'radialGauge',
  data: {{
    datasets: [{{
      data: [{percentage}],
      backgroundColor: ""{color}""
    }}]
  }}
}}
";

            return chart;
        }

        private Chart PieChart(IEnumerable<(string, int)> data)
        {
            Chart chart = new Chart();

            var labels = string.Join(",", data.Select(d => $"'{d.Item1}'"));
            var dataSet = string.Join(",", data.Select(d => $"{d.Item2}"));

            chart.Config = $@"
{{
  type: 'pie',
  data: {{
    labels: [{labels}],
    datasets: [{{
      data: [{dataSet}]
    }}]
  }}
}}
";

            return chart;
        }
    }
}