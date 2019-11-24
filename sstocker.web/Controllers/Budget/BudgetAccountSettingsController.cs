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
                var settings = AccountRepository.GetAccountSettings<CategorySetting>(sharedAccountId, SettingsHelper.CategorySettingKey);
                var model = new AccountSettingsViewModel(settings, true, hasSharedAccount);
                model.SetBaseViewModel(accountId);
                return View(SettingsHelper.GetAccountControllerViewPath("BudgetAccountSettings"), model);
            }
            else
            {
                var settings = AccountRepository.GetAccountSettings<CategorySetting>(accountId, SettingsHelper.CategorySettingKey);
                var model = new AccountSettingsViewModel(settings, false, hasSharedAccount);
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
            var categories = CategoryHelper.GetCategories();

            foreach (var setting in settings)
            {
                var split = setting.Split(",");
                var name = split[0];
                var categoryId = categories.Single(c => c.Name == name).CategoryId;
                var categorySetting = new CategorySetting
                {
                    IsActive = bool.Parse(split[1]),
                    IsCritical = bool.Parse(split[2]),
                    Unlimited = bool.Parse(split[3]),
                    Amount = long.Parse(split[4]),
                    Duration = (Duration)Enum.Parse(typeof(Duration), split[5])
                };

                AccountRepository.AddOrUpdateAccountSetting(savingAccountId, SettingsHelper.CategorySettingKey, categoryId, categorySetting);
            }

            return Json(new { status = true, message = "Settings Saved" });
        }
    }
}
