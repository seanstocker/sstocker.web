using Microsoft.AspNetCore.Mvc;
using sstocker.budget.Enums;
using sstocker.budget.Helpers;
using sstocker.budget.Models;
using sstocker.budget.Repositories;
using sstocker.budget.ViewModels;
using sstocker.core.Helpers;
using System;
using System.Linq;

namespace sstocker.web.Controllers
{
    [Route(SettingsHelper.ControllerRoute)]
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View(SettingsHelper.GetAccountControllerViewPath("Login"));
        }

        public IActionResult AddAccount()
        {
            return View(SettingsHelper.GetAccountControllerViewPath("AddAccount"));
        }

        public IActionResult Settings(bool shared)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

            var hasSharedAccount = AccountHelper.HasSharedAccount(accountId);

            if (hasSharedAccount && shared)
            {
                var sharedAccountId = AccountHelper.GetSharedAccountId(accountId);
                var settings = AccountRepository.GetAccountSettings<CategorySetting>(sharedAccountId, SettingsHelper.CategorySettingKey);
                var model = new AccountSettingsViewModel(settings, true, hasSharedAccount);
                model.SetBaseViewModel(accountId);
                return View(SettingsHelper.GetAccountControllerViewPath("Settings"), model);
            }
            else
            {
                var settings = AccountRepository.GetAccountSettings<CategorySetting>(accountId, SettingsHelper.CategorySettingKey);
                var model = new AccountSettingsViewModel(settings, false, hasSharedAccount);
                model.SetBaseViewModel(accountId);
                return View(SettingsHelper.GetAccountControllerViewPath("Settings"), model);
            }
        }

        public ActionResult Validate(string username, string password)
        {
            var account = AccountRepository.GetAccountWithPassword(username);

            if (account != null && account != default(AccountWithPassword))
            {
                if (account.Password == PasswordHelper.ComputeSaltedHash(password, account.Salt))
                {
                    HttpContext.Session.Set(SessionHelper.SessionKeyAccountId, account.AccountId);
                    return Json(new { status = true, message = "Login Successfull!" });
                }
                else
                {
                    return Json(new { status = false, message = "Invalid Password" });
                }
            }
            else
            {
                return Json(new { status = false, message = "Invalid Username" });
            }
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Set(SessionHelper.SessionKeyAccountId, 0);
            return RedirectToAction("Dashboard", "Home");
        }

        public ActionResult GetName(string username)
        {
            var account = AccountRepository.GetAccount(username);

            if (account != null && account != default(Account) && !string.IsNullOrWhiteSpace(account.Name))
                return Json(new { status = true, name = account.Name });
            else
                return Json(new { status = false });
        }

        public ActionResult CreateAccount(string username, string password, string name)
        {
            username = username.Trim();
            if (string.IsNullOrWhiteSpace(username))
                return Json(new { status = false, message = "Failed to create user. Username cannot be empty." });
            if (string.IsNullOrWhiteSpace(password))
                return Json(new { status = false, message = "Failed to create user. Password cannot be empty." });
            if (string.IsNullOrWhiteSpace(name))
                return Json(new { status = false, message = "Failed to create user. Name cannot be empty." });

            var account = AccountRepository.GetAccount(username);

            if (account != null && account != default(Account))
                return Json(new { status = false, message = "Failed to create user. Username already exists." });

            var salt = PasswordHelper.CreateRandomSalt();
            var saltedPassword = PasswordHelper.ComputeSaltedHash(password, salt);

            AccountRepository.CreateAccount(username, saltedPassword, salt.ToString(), name);
            return Json(new { status = true, message = $"User {username} created." });
        }

        public ActionResult SaveSettings(string model, bool isSharedAccount)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

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
