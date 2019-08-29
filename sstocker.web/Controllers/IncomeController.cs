using Microsoft.AspNetCore.Mvc;
using sstocker.budget.Helpers;
using sstocker.budget.Repositories;
using sstocker.budget.ViewModels;
using sstocker.core.Helpers;
using System;

namespace sstocker.web.Controllers
{
    [Route(SettingsHelper.ControllerRoute)]
    public class IncomeController : Controller
    {
        public IActionResult AddIncome()
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

            var model = new AddIncomeModel(accountId);
            model.SetBaseViewModel(accountId);
            return View(SettingsHelper.GetIncomeControllerViewPath("AddIncome"), model);
        }

        public ActionResult SaveIncome(string source, string type, string amount, string date)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

            decimal amountValue;
            DateTime dateValue;

            if (string.IsNullOrWhiteSpace(source))
                return Json(new { status = false, message = "Source is required" });
            if (string.IsNullOrWhiteSpace(type))
                return Json(new { status = false, message = "Type is required" });
            if (string.IsNullOrWhiteSpace(amount))
                return Json(new { status = false, message = "Amount is required" });
            if (!decimal.TryParse(amount, out amountValue))
                return Json(new { status = false, message = "Amount is required" });
            if (string.IsNullOrWhiteSpace(date))
                return Json(new { status = false, message = "Date is required" });
            if (!DateTime.TryParse(date, out dateValue))
                return Json(new { status = false, message = "Date is required" });

            var sourceId = IncomeSourceHelper.GetOrAddIncomeSourceId(source);
            var typeId = IncomeTypeHelper.GetIncomeTypeId(type);

            IncomeRepository.AddIncome(accountId, sourceId, typeId, amountValue, dateValue);
            return Json(new { status = true, message = "Income Added" });
        }
    }
}
