using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sstocker.budget.Enums;
using sstocker.budget.Helpers;
using sstocker.budget.Models;
using sstocker.budget.ViewModels;
using sstocker.core.Helpers;
using sstocker.core.Repositories;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace sstocker.web.Controllers.Budget
{
    [Route(SettingsHelper.ControllerRoute)]
    public class BudgetAccountSettingsController : Controller
    {
        public IActionResult Settings(bool shared)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.BudgetApp });

            var hasSharedAccount = AccountHelper.HasSharedAccount(accountId);

            if (hasSharedAccount && shared)
            {
                var sharedAccountId = AccountHelper.GetSharedAccountId(accountId);
                var categorySettings = SettingsHelper.GetCategorySettings(sharedAccountId);
                var expenseSummarySettings = SettingsHelper.GetExpenseSummarySettings(sharedAccountId);
                var emailSettings = SettingsHelper.GetEmailSettings(sharedAccountId);
                var model = new AccountSettingsViewModel(categorySettings, expenseSummarySettings, emailSettings, true, hasSharedAccount);
                model.SetBaseViewModel(accountId);
                return View(SettingsHelper.GetAccountControllerViewPath("BudgetAccountSettings"), model);
            }
            else
            {
                var categorySettings = SettingsHelper.GetCategorySettings(accountId);
                var expenseSummarySettings = SettingsHelper.GetExpenseSummarySettings(accountId);
                var emailSettings = SettingsHelper.GetEmailSettings(accountId);
                var model = new AccountSettingsViewModel(categorySettings, expenseSummarySettings, emailSettings, false, hasSharedAccount);
                model.SetBaseViewModel(accountId);
                return View(SettingsHelper.GetAccountControllerViewPath("BudgetAccountSettings"), model);
            }
        }

        public ActionResult SaveSettings(string model, bool isSharedAccount)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.BudgetApp });

            var savingAccountId = isSharedAccount ? AccountHelper.GetSharedAccountId(accountId) : accountId;

            var settings = model.Split("|");

            foreach (var setting in settings)
            {
                string contextKey;
                string contextValue;
                object settingValue;

                if (IsValidSettingString(setting, "CATEGORY"))
                {
                    contextKey = SettingsHelper.CategorySettingKey;
                    contextValue = GetCategorySettingContextValue(setting);
                    settingValue = ParseCategorySetting(setting);
                }
                else if (IsValidSettingString(setting, "EXPENSESUMMARYTIMEPERIOD"))
                {
                    contextKey = SettingsHelper.ExpenseSummarySettingKey;
                    contextValue = SettingsHelper.TimePeriodSettingValue;
                    settingValue = ParseExpenseSummarySetting(setting);
                }
                else if (IsValidSettingString(setting, "EMAIL"))
                {
                    contextKey = SettingsHelper.EmailSettingKey;
                    contextValue = SettingsHelper.EmailSettingValue;
                    settingValue = ParseEmailSetting(setting);
                }
                else
                {
                    throw new NotImplementedException();
                }

                if (settingValue != null)
                {
                    AccountRepository.AddOrUpdateAccountSetting(savingAccountId, contextKey, contextValue, settingValue);
                }
            }

            return Json(new { status = true, message = "Settings Saved" });
        }

        private bool IsValidSettingString(string settingString, string type)
        {
            return settingString.StartsWith($"--{type}--");
        }

        private string GetValidSettingString(string settingString, string type)
        {
            if (!IsValidSettingString(settingString, type))
            {
                throw new Exception("Cannot parse string. Invalid format");
            }

            return settingString.Replace($"--{type}--", "");
        }

        private CategorySetting ParseCategorySetting(string settingString)
        {
            settingString = GetValidSettingString(settingString, "CATEGORY");
            var split = settingString.Split(',');

            var categorySetting = new CategorySetting
            {
                IsActive = bool.Parse(split[1]),
                IsCritical = bool.Parse(split[2]),
                Unlimited = bool.Parse(split[3]),
                Amount = long.Parse(split[4]),
                Duration = (Duration)Enum.Parse(typeof(Duration), split[5])
            };

            return categorySetting;
        }

        private string GetCategorySettingContextValue(string settingString)
        {
            settingString = GetValidSettingString(settingString, "CATEGORY");
            var split = settingString.Split(',');
            var name = split[0];
            var categoryId = CategoryHelper.GetCategories().Single(c => c.Name == name).CategoryId;
            return categoryId.ToString();
        }

        private ExpenseSummaryTimePeriod ParseExpenseSummarySetting(string settingString)
        {
            settingString = GetValidSettingString(settingString, "EXPENSESUMMARYTIMEPERIOD");
            return (ExpenseSummaryTimePeriod)Enum.Parse(typeof(ExpenseSummaryTimePeriod), settingString);
        }

        private EmailSettings ParseEmailSetting(string settingString)
        {
            settingString = GetValidSettingString(settingString, "EMAIL");
            var split = settingString.Split(',');

            if(split.Length != 4)
            {
                return null;
            }

            if(!IsValidEmail(split[3]))
            {
                return null;
            }

            return new EmailSettings
            {
                SendWeeklyEmail = bool.Parse(split[0]),
                SendMonthlyEmail = bool.Parse(split[1]),
                SendReminderEmail = bool.Parse(split[2]),
                Email = split[3]
            };
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
