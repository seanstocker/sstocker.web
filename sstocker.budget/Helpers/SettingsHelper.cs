using sstocker.budget.Enums;
using sstocker.budget.Models;
using sstocker.core.Models;
using sstocker.core.Repositories;
using System.Linq;

namespace sstocker.budget.Helpers
{
    public static class SettingsHelper
    {
        public const string CategorySettingKey = "CATEGORY";
        public const string ExpenseSummarySettingKey = "EXPENSESUMMARY";
        public const string TimePeriodSettingValue = "TIMEPERIOD";
        public const string ControllerRoute = "Budget/[controller]/[action]";

        public static string GetAccountControllerViewPath(string action = "Index")
        {
            return GetViewPath("Account", action);
        }

        public static string GetExpenseControllerViewPath(string action = "Index")
        {
            return GetViewPath("Expense", action);
        }

        public static string GetBudgetHomeControllerViewPath(string action = "Index")
        {
            return GetViewPath("BudgetHome", action);
        }

        public static string GetIncomeControllerViewPath(string action = "Index")
        {
            return GetViewPath("Income", action);
        }

        public static string GetSnapshotControllerViewPath(string action = "Index")
        {
            return GetViewPath("Snapshot", action);
        }

        private static string GetViewPath(string controller, string action)
        {
            return $"Views/Budget/{controller}/{action}.cshtml";
        }

        public static AccountSettings<CategorySetting> GetCategorySettings(long accountId)
        {
            return AccountRepository.GetAccountSettings<CategorySetting>(accountId, CategorySettingKey);
        }

        public static AccountSettings<ExpenseSummaryTimePeriod> GetExpenseSummarySettings(long accountId)
        {
            return AccountRepository.GetAccountSettings<ExpenseSummaryTimePeriod>(accountId, ExpenseSummarySettingKey);
        }

        public static ExpenseSummaryTimePeriod? GetTimePeriod(long accountId)
        {
            return GetTimePeriod(GetExpenseSummarySettings(accountId));
        }

        public static ExpenseSummaryTimePeriod? GetTimePeriod(AccountSettings<ExpenseSummaryTimePeriod> settings)
        {
            return settings.Settings.SingleOrDefault(s => s.ContextValue == TimePeriodSettingValue)?.Data;
        }
    }
}