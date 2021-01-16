using Microsoft.AspNetCore.Mvc;
using sstocker.budget.Helpers;
using sstocker.budget.Repositories;
using sstocker.budget.ViewModels;
using sstocker.core.Helpers;
using System;

namespace sstocker.web.Controllers.Budget
{
    [Route(SettingsHelper.ControllerRoute)]
    public class SnapshotController : Controller
    {
        public IActionResult AddSnapshot()
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.BudgetApp });

            var model = new AddSnapshotModel();
            model.SetBaseViewModel(accountId);
            return View(SettingsHelper.GetSnapshotControllerViewPath("AddSnapshot"), model);
        }

        public ActionResult SaveSnapshot(string bank, string bankType, string amount, string date)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.BudgetApp });

            if (string.IsNullOrWhiteSpace(bank))
                return Json(new { status = false, message = "Bank is required" });
            if (string.IsNullOrWhiteSpace(bankType))
                return Json(new { status = false, message = "Bank Type is required" });
            if (string.IsNullOrWhiteSpace(amount))
                return Json(new { status = false, message = "Amount is required" });
            if (!decimal.TryParse(amount, out decimal amountValue))
                return Json(new { status = false, message = "Amount is required" });
            if (string.IsNullOrWhiteSpace(date))
                return Json(new { status = false, message = "Date is required" });
            if (!DateTime.TryParse(date, out DateTime dateValue))
                return Json(new { status = false, message = "Date is required" });

            var bankId = BankHelper.GetOrAddBankId(bank);
            var bankTypeId = BankHelper.GetBankTypeId(bankType);

            SnapshotRepository.AddSnapshot(accountId, bankId, bankTypeId, amountValue, dateValue);
            return Json(new { status = true, message = "Snapshot Added" });
        }

        public ActionResult SnapshotSummary()
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.BudgetApp });

            var model = new SnapshotSummaryModel(accountId);
            model.SetBaseViewModel(accountId);
            return View(SettingsHelper.GetSnapshotControllerViewPath("SnapshotSummary"), model);
        }
    }
}