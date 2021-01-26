using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sstocker.budget.Enums;
using sstocker.budget.Helpers;
using sstocker.budget.Models;
using sstocker.budget.ViewModels;
using sstocker.core.Helpers;
using sstocker.core.Repositories;
using System;
using System.Linq;

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
                var model = new AccountSettingsViewModel(categorySettings, expenseSummarySettings, true, hasSharedAccount);
                model.SetBaseViewModel(accountId);
                return View(SettingsHelper.GetAccountControllerViewPath("BudgetAccountSettings"), model);
            }
            else
            {
                var categorySettings = SettingsHelper.GetCategorySettings(accountId);
                var expenseSummarySettings = SettingsHelper.GetExpenseSummarySettings(accountId);
                var model = new AccountSettingsViewModel(categorySettings, expenseSummarySettings, false, hasSharedAccount);
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
                else
                {
                    throw new NotImplementedException();
                }

                AccountRepository.AddOrUpdateAccountSetting(savingAccountId, contextKey, contextValue, settingValue);
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
    }
}
