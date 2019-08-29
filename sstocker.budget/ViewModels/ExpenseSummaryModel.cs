using sstocker.budget.Enums;
using sstocker.budget.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sstocker.budget.ViewModels
{
    public class ExpenseSummaryModel : BaseViewModel
    {
        public List<string> ColumnNames;
        public List<Expense> Expenses;
        public ExpenseSummaryTimePeriod TimePeriod;
        public DateTime StartDate;
        public DateTime EndDate;
        public bool HasSharedAccount;

        public ExpenseSummaryModel(List<Expense> expenses, ExpenseSummaryTimePeriod timePeriod, bool hasSharedAccount)
        {
            TimePeriod = timePeriod;
            var min = expenses.Any() ? (DateTime?)expenses.Min(e => e.SpentDate) : null;
            var max = expenses.Any() ? (DateTime?)expenses.Max(e => e.SpentDate) : null;
            StartDate = timePeriod.GetStartDate(min);
            EndDate = timePeriod.GetEndDate(max);
            Expenses = expenses.Where(e => e.SpentDate >= StartDate && e.SpentDate <= EndDate.AddDays(1).AddSeconds(-1)).ToList();
            ColumnNames = new List<string> { "Store", "Category", "Amount", "SpentDate" };
            HasSharedAccount = hasSharedAccount;
        }

        public string GetExpenseColumn(int expenseLocation, int columnLocation)
        {
            switch (columnLocation)
            {
                case 0:
                    return Expenses[expenseLocation].Store;
                case 1:
                    return Expenses[expenseLocation].Category;
                case 2:
                    return Expenses[expenseLocation].Amount.ToString("$0.00");
                case 3:
                    return Expenses[expenseLocation].SpentDate.ToShortDateString();
                default:
                    throw new Exception("Not a valid column location.");
            }
        }
    }
}
