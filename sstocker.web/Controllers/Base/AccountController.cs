using Microsoft.AspNetCore.Mvc;
using sstocker.budget.ViewModels;
using sstocker.core.Helpers;
using sstocker.core.Models;
using sstocker.core.Repositories;
using sstocker.core.ViewModels;
using System.Text.RegularExpressions;

namespace sstocker.web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login(string id)
        {
            return View(new LoginViewModel(id));
        }

        public IActionResult AddAccount()
        {
            return View();
        }

        public IActionResult Profile()
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.BudgetApp });

            var model = new ProfileViewModel(accountId);
            model.SetBaseViewModel(accountId);

            return View(model);
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
            var account = AccountRepository.GetAccount(username, true);

            if (account != null && account != default(Account) && !string.IsNullOrWhiteSpace(account.Name))
                return Json(new { name = account.Name, image = account.Image });
            else
                return Json(new { name = "John Doe", image = AccountHelper.GetImage() });
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

        public ActionResult DeleteProfileImage()
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.BudgetApp });

            AccountRepository.DeleteAccountImage(accountId);

            return Json(new { status = true });
        }

        public ActionResult UpdateProfileImage(string image)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.BudgetApp });

            if (!string.IsNullOrWhiteSpace(image))
                AccountRepository.UpdateAccountImage(accountId, Regex.Replace(image, @"data:image\/.*?;base64,", ""));

            return Json(new { status = true });
        }
    }
}
