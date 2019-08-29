﻿namespace sstocker.budget.Helpers
{
    public static class SettingsHelper
    {
        public const string CategorySettingKey = "CATEGORY";
        public const string ControllerRoute = "Budget/[controller]/[action]";

        public static string GetAccountControllerViewPath(string action = "Index")
        {
            return GetViewPath("Account", action);
        }

        public static string GetExpenseControllerViewPath(string action = "Index")
        {
            return GetViewPath("Expense", action);
        }

        public static string GetHomeControllerViewPath(string action = "Index")
        {
            return GetViewPath("Home", action);
        }

        public static string GetIncomeControllerViewPath(string action = "Index")
        {
            return GetViewPath("Income", action);
        }

        public static string GetViewPath(string controller, string action)
        {
            return $"Views/Budget/{controller}/{action}.cshtml";
        }
    }
}
