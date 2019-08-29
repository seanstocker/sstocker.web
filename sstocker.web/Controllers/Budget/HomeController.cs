using Microsoft.AspNetCore.Mvc;
using sstocker.budget.Enums;
using sstocker.budget.Helpers;
using sstocker.budget.Models;
using sstocker.budget.Repositories;
using sstocker.budget.ViewModels;
using sstocker.core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sstocker.web.Controllers
{
    [Route(SettingsHelper.ControllerRoute)]
    public class HomeController : Controller
    {
        public IActionResult Dashboard(bool shared)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default(long))
                return RedirectToAction("Login", "Account");

            if (shared)
            {
                var sharedAccountId = AccountHelper.GetSharedAccountId(accountId);
                var model = GetDashboardModel(accountId, sharedAccountId, true);
                model.SetBaseViewModel(accountId);
                return View(SettingsHelper.GetHomeControllerViewPath("Dashboard"), model);
            }
            else
            {
                var sharedAccountId = AccountHelper.HasSharedAccount(accountId) ? (long?)AccountHelper.GetSharedAccountId(accountId) : null;
                var model = GetDashboardModel(accountId, sharedAccountId, false);
                model.SetBaseViewModel(accountId);
                return View(SettingsHelper.GetHomeControllerViewPath("Dashboard"), model);
            }
        }

        private DashboardModel GetDashboardModel(long accountId, long? sharedAccountId, bool sharedDashboard)
        {
            var model = new DashboardModel();

            var expenses = sharedDashboard
                ? ExpenseRepository.GetAccountExpenses(sharedAccountId.GetValueOrDefault())
                : ExpenseRepository.GetAccountExpenses(accountId);
            var income = sharedDashboard
                ? IncomeRepository.GetAccountIncome(sharedAccountId.GetValueOrDefault())
                : IncomeRepository.GetAccountIncome(accountId);

            model.SpentToday = GetTotalSpentAmount(expenses, ExpenseSummaryTimePeriod.Today);
            model.SpentThisWeek = GetTotalSpentAmount(expenses, ExpenseSummaryTimePeriod.ThisWeek);
            model.SpentThisMonth = GetTotalSpentAmount(expenses, ExpenseSummaryTimePeriod.ThisMonth);
            model.SpentThisYear = GetTotalSpentAmount(expenses, ExpenseSummaryTimePeriod.ThisYear);

            model.ExpensesLineChart = GetLineChart(expenses);
            model.IncomeLineChart = GetLineChart(income);
            model.PieChart = GetPieChart(expenses);

            model.BudgetOverviewList = sharedDashboard
                ? GetBudgetOverviewChart(expenses, sharedAccountId.GetValueOrDefault())
                : GetBudgetOverviewChart(expenses, accountId);

            model.Shared = GetSharedDashboardModel(accountId, sharedAccountId, sharedDashboard);

            return model;
        }

        public decimal GetTotalSpentAmount(List<Expense> expenses, ExpenseSummaryTimePeriod timePeriod)
        {
            var startDate = timePeriod.GetStartDate();
            var endDate = timePeriod.GetEndDate();
            return GetTotalSpentAmount(expenses, startDate, endDate);
        }

        public decimal GetTotalSpentAmount(List<Income> income, ExpenseSummaryTimePeriod timePeriod)
        {
            var startDate = timePeriod.GetStartDate();
            var endDate = timePeriod.GetEndDate();
            return GetTotalSpentAmount(income, startDate, endDate);
        }

        public decimal GetTotalSpentAmount(List<Expense> expenses, DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;
            expenses = expenses.Where(e => e.SpentDate >= startDate && e.SpentDate <= endDate.AddDays(1).AddSeconds(-1)).ToList();
            return expenses.Sum(e => e.Amount);
        }

        public decimal GetTotalSpentAmount(List<Income> income, DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;
            income = income.Where(e => e.IncomeDate >= startDate && e.IncomeDate <= endDate.AddDays(1).AddSeconds(-1)).ToList();
            return income.Sum(e => e.Amount);
        }

        public ChartJSChartModel GetLineChart(List<Expense> expenses)
        {
            var today = DateTime.UtcNow.AddHours(-6).Date;
            var model = new ChartJSChartModel();

            for (var x = 0; x < 12; x++)
            {
                var startDate = new DateTime(today.AddMonths(-x).Year, today.AddMonths(-x).Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var monthName = today.AddMonths(-x).ToString("MMM");
                var amount = GetTotalSpentAmount(expenses, startDate, endDate);
                model.AddPointBeginning(monthName, amount);
            }

            return model;
        }

        public ChartJSChartModel GetLineChart(List<Income> income)
        {
            var today = DateTime.UtcNow.AddHours(-6).Date;
            var model = new ChartJSChartModel();

            for (var x = 0; x < 12; x++)
            {
                var startDate = new DateTime(today.AddMonths(-x).Year, today.AddMonths(-x).Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var monthName = today.AddMonths(-x).ToString("MMM");
                var amount = GetTotalSpentAmount(income, startDate, endDate);
                model.AddPointBeginning(monthName, amount);
            }

            return model;
        }

        public ChartJSChartModel GetPieChart(List<Expense> expenses)
        {
            expenses = expenses.Where(e => e.SpentDate >= ExpenseSummaryTimePeriod.ThisMonth.GetStartDate() && e.SpentDate <= ExpenseSummaryTimePeriod.ThisMonth.GetEndDate().AddDays(1).AddSeconds(-1)).ToList();
            var model = new ChartJSChartModel();
            var categories = expenses.GroupBy(e => e.Category).OrderByDescending(e => e.Sum(e2 => e2.Amount));

            foreach (var category in categories)
                model.AddPoint(category.Key, category.Sum(e => e.Amount));

            return model;
        }

        public List<BudgetOverview> GetBudgetOverviewChart(List<Expense> expenses, long accountId)
        {
            var model = new List<BudgetOverview>();
            var settings = AccountRepository.GetAccountSettings<CategorySetting>(accountId, SettingsHelper.CategorySettingKey);
            var categories = CategoryHelper.GetCategories();

            foreach (var setting in settings.Settings)
            {
                if (!setting.Data.IsActive)
                    continue;
                if (setting.Data.Unlimited)
                    continue;
                if (setting.Data.Amount <= 0)
                    continue;

                var startDate = setting.Data.Duration.GetStartDate();
                var endDate = setting.Data.Duration.GetEndDate();
                var name = categories.Single(c => c.CategoryId == setting.ContextValue).Name;

                var categoryExpenses = expenses.Where(e => e.Category == name
                    && e.SpentDate >= startDate
                    && e.SpentDate <= endDate.AddDays(1).AddMilliseconds(-1)).ToList();
                var spentTotal = categoryExpenses.Sum(ce => ce.Amount);

                model.Add(new BudgetOverview(name, setting.Data.Duration.ToString(), spentTotal, setting.Data.Amount));
            }

            return model;
        }

        public SharedDashboardModel GetSharedDashboardModel(long accountId, long? sharedAccountId, bool sharedDashboard)
        {
            var hasSharedAccount = sharedAccountId != null;
            var partnerName = !sharedDashboard && hasSharedAccount ? AccountRepository.GetPartnerName(accountId) : null;
            var model = new SharedDashboardModel(hasSharedAccount, sharedDashboard, partnerName);

            if (!sharedDashboard && hasSharedAccount)
            {
                var thisMonth = ExpenseSummaryTimePeriodHelper.GetStartDate(ExpenseSummaryTimePeriod.ThisMonth);
                var lastMonth = ExpenseSummaryTimePeriodHelper.GetStartDate(ExpenseSummaryTimePeriod.LastMonth);
                model.AddSharedOweAmount(GetOweAmount(accountId, sharedAccountId.GetValueOrDefault(), thisMonth), thisMonth);
                model.AddSharedOweAmount(GetOweAmount(accountId, sharedAccountId.GetValueOrDefault(), lastMonth), lastMonth);
            }

            return model;
        }

        private decimal GetOweAmount(long accountId, long sharedAccountId, DateTime month)
        {
            var expenses = ExpenseRepository.GetAccountExpenses(sharedAccountId);
            expenses = expenses.Where(e => e.SpentDate.Year == month.Year && e.SpentDate.Month == month.Month).ToList();

            var yourIncomeTotal = IncomeRepository.GetAccountIncome(accountId).Where(i => i.IncomeDate.Year == month.Year && i.IncomeDate.Month == month.Month).Sum(i => i.Amount);
            var partnerIncomeTotal = IncomeRepository.GetPartnerIncome(accountId).Where(i => i.IncomeDate.Year == month.Year && i.IncomeDate.Month == month.Month).Sum(i => i.Amount);
            var yourPercentage = yourIncomeTotal + partnerIncomeTotal == 0 ? 0 : yourIncomeTotal / (yourIncomeTotal + partnerIncomeTotal);

            var sharedSpentTotal = expenses.Sum(e => e.Amount);
            var yourSharedSpentTotal = expenses.Where(e => e.SpentAccountId == accountId).Sum(e => e.Amount);
            var yourIdealSpentTotal = yourPercentage * sharedSpentTotal;

            return Math.Round(yourIdealSpentTotal - yourSharedSpentTotal, 2);
        }
    }
}
