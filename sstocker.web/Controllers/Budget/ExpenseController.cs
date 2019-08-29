using Microsoft.AspNetCore.Http;
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
    public class ExpenseController : Controller
    {
        public IActionResult ExpenseSummary(ExpenseSummaryTimePeriod TimePeriod)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

            var model = GetExpenseSummary(accountId, TimePeriod);
            model.SetBaseViewModel(accountId);
            return View(SettingsHelper.GetExpenseControllerViewPath("ExpenseSummary"), model);
        }

        public IActionResult SharedExpenseSummary(ExpenseSummaryTimePeriod TimePeriod)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

            var sharedAccountId = AccountHelper.GetSharedAccountId(accountId);
            var model = GetExpenseSummary(sharedAccountId, TimePeriod);
            model.SetBaseViewModel(accountId);
            return View(SettingsHelper.GetExpenseControllerViewPath("SharedExpenseSummary"), model);
        }

        public IActionResult Summary(SummaryTimePeriod? TimePeriod)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

            var model = GetSummaryModel(accountId, TimePeriod ?? SummaryTimePeriod.Past30Days);
            model.SetBaseViewModel(accountId);
            return View(SettingsHelper.GetExpenseControllerViewPath("Summary"), model);
        }

        public IActionResult BudgetSummary()
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

            var model = GetBudgetSummaryModel(accountId);
            model.SetBaseViewModel(accountId);
            return View(SettingsHelper.GetExpenseControllerViewPath("BudgetSummary"), model);
        }

        public IActionResult AddExpense()
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

            var model = new AddExpenseModel(accountId);
            model.SetBaseViewModel(accountId);
            return View(SettingsHelper.GetExpenseControllerViewPath("AddExpense"), model);
        }

        public IActionResult EditExpense(Guid id)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

            var model = new EditExpenseModel(accountId, id);
            model.SetBaseViewModel(accountId);
            return View(SettingsHelper.GetExpenseControllerViewPath("EditExpense"), model);
        }

        public IActionResult DeleteExpense(Guid id)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

            if (id == default(Guid) || id == Guid.Empty)
                return RedirectToAction("ExpenseSummary", "Expense");

            ExpenseRepository.DeleteExpense(id);

            return RedirectToAction("ExpenseSummary", "Expense");
        }

        public ActionResult SaveExpense(string store, string category, string amount, string date, bool sharedExpense)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

            decimal amountValue;
            DateTime dateValue;

            if (string.IsNullOrWhiteSpace(store))
                return Json(new { status = false, message = "Store is required" });
            if (string.IsNullOrWhiteSpace(category))
                return Json(new { status = false, message = "Category is required" });
            if (string.IsNullOrWhiteSpace(amount))
                return Json(new { status = false, message = "Amount is required" });
            if (!decimal.TryParse(amount, out amountValue))
                return Json(new { status = false, message = "Amount is required" });
            if (string.IsNullOrWhiteSpace(date))
                return Json(new { status = false, message = "Date is required" });
            if (!DateTime.TryParse(date, out dateValue))
                return Json(new { status = false, message = "Date is required" });

            var storeId = StoreHelper.GetOrAddStoreId(store);
            var categoryId = CategoryHelper.GetCategoryId(category);

            if (sharedExpense)
            {
                var sharedAccountId = AccountHelper.GetSharedAccountId(accountId);
                ExpenseRepository.AddExpense(sharedAccountId, storeId, categoryId, amountValue, dateValue, accountId);
            }
            else
            {
                ExpenseRepository.AddExpense(accountId, storeId, categoryId, amountValue, dateValue, accountId);
            }
            return Json(new { status = true, message = "Expense Added" });
        }

        public ActionResult SaveEditExpense(Guid id, string store, string category, string amount, string date, bool sharedExpense)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

            decimal amountValue;
            DateTime dateValue;

            if (id == default(Guid) || id == Guid.Empty)
                return Json(new { status = false, message = "ERROR: Please refresh the page." });
            if (string.IsNullOrWhiteSpace(store))
                return Json(new { status = false, message = "Store is required" });
            if (string.IsNullOrWhiteSpace(category))
                return Json(new { status = false, message = "Category is required" });
            if (string.IsNullOrWhiteSpace(amount))
                return Json(new { status = false, message = "Amount is required" });
            if (!decimal.TryParse(amount, out amountValue))
                return Json(new { status = false, message = "Amount is required" });
            if (string.IsNullOrWhiteSpace(date))
                return Json(new { status = false, message = "Date is required" });
            if (!DateTime.TryParse(date, out dateValue))
                return Json(new { status = false, message = "Date is required" });

            var storeId = StoreHelper.GetOrAddStoreId(store);
            var categoryId = CategoryHelper.GetCategoryId(category);

            if (sharedExpense)
            {
                var sharedAccountId = AccountHelper.GetSharedAccountId(accountId);
                ExpenseRepository.EditExpense(id, sharedAccountId, storeId, categoryId, amountValue, dateValue);
            }
            else
            {
                ExpenseRepository.EditExpense(id, accountId, storeId, categoryId, amountValue, dateValue);
            }
            return Json(new { status = true, message = "Expense Editted" });
        }

        public ExpenseSummaryModel GetExpenseSummary(long accountId, ExpenseSummaryTimePeriod timePeriod)
        {
            var expenses = ExpenseRepository.GetAccountExpenses(accountId);
            var hasSharedAccount = AccountHelper.HasSharedAccount(accountId);
            var model = new ExpenseSummaryModel(expenses, timePeriod, hasSharedAccount);
            return model;
        }

        private SummaryModel GetSummaryModel(long accountId, SummaryTimePeriod timePeriod)
        {
            var expenses = ExpenseRepository.GetAccountExpenses(accountId);
            var settings = AccountRepository.GetAccountSettings<CategorySetting>(accountId, SettingsHelper.CategorySettingKey);
            var model = new SummaryModel(timePeriod, expenses.Min(e => e.SpentDate), expenses.Max(e => e.SpentDate));
            expenses = expenses.Where(e => e.SpentDate >= model.StartDate && e.SpentDate <= model.EndDate.AddDays(1).AddSeconds(-1)).ToList();

            return model;
        }

        private BudgetSummaryModel GetBudgetSummaryModel(long accountId)
        {
            var model = new BudgetSummaryModel();
            model = AddBudgetSummaryModelSettings(model, accountId, false);

            if (AccountHelper.HasSharedAccount(accountId))
            {
                var sharedAccountId = AccountHelper.GetSharedAccountId(accountId);
                model = AddBudgetSummaryModelSettings(model, sharedAccountId, true);
            }

            return model;
        }

        private BudgetSummaryModel AddBudgetSummaryModelSettings(BudgetSummaryModel model, long accountId, bool sharedAccount)
        {
            var categories = CategoryHelper.GetCategories();
            var expenses = ExpenseRepository.GetAccountExpenses(accountId);
            var settings = AccountRepository.GetAccountSettings<CategorySetting>(accountId, SettingsHelper.CategorySettingKey);

            foreach (var setting in settings.Settings)
            {
                if (!setting.Data.IsActive)
                    continue;
                if (setting.Data.Unlimited)
                    continue;
                if (setting.Data.Amount <= 0)
                    continue;

                model.AddCategoryTable(setting.Data.Duration, categories.Single(c => c.CategoryId == setting.ContextValue), setting.Data.Amount, expenses, sharedAccount);
            }

            return model;
        }
    }
}
