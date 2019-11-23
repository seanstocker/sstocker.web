using sstocker.budget.Enums;
using sstocker.budget.Models;
using sstocker.core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sstocker.budget.ViewModels
{
    public class BudgetSummaryModel : BaseViewModel
    {
        public List<BudgetSummaryModelTable> CategoryTables;

        public BudgetSummaryModel()
        {
            CategoryTables = new List<BudgetSummaryModelTable>();
        }

        public void AddCategoryTable(Duration duration, Category category, long settingsAmount, List<Expense> expenses, bool sharedBudget)
        {
            var table = new BudgetSummaryModelTable(duration, category, sharedBudget);
            var categoryExpenses = expenses.Where(e => e.Category == table.Category.Name
                && e.SpentDate >= table.StartDate
                && e.SpentDate <= table.EndDate.AddDays(1).AddMilliseconds(-1)).ToList();
            var totalAmount = categoryExpenses.Sum(ce => ce.Amount);

            foreach (var e in categoryExpenses)
            {
                var percentage = e.Amount / totalAmount;
                var expireDate = table.DurationDuration - (DateTime.Now.Date - e.SpentDate).Days + 1;
                table.Rows.Add(new BudgetSummaryModelTableRow(e.Store, e.Amount, e.SpentDate, percentage, expireDate));
            }

            table.TotalRow = new BudgetSummaryModelTableRow("TOTAL", totalAmount, new DateTime(), 1, 0);
            table.LeftRow = new BudgetSummaryModelTableRow("REMAINING BALANCE", settingsAmount - totalAmount, new DateTime(), 1 - (totalAmount / settingsAmount), 0);

            CategoryTables.Add(table);
        }

        public List<string> GetTableNames()
        {
            return CategoryTables.Select(t => t.TableId).ToList();
        }
    }

    public class BudgetSummaryModelTable
    {
        public Duration Duration;
        public Category Category;
        public DateTime StartDate;
        public DateTime EndDate;
        public long DurationDuration;
        public List<BudgetSummaryModelTableRow> Rows;
        public BudgetSummaryModelTableRow TotalRow;
        public BudgetSummaryModelTableRow LeftRow;
        public bool SharedBudget;
        public string TableId;

        public BudgetSummaryModelTable(Duration duration, Category category, bool sharedBudget)
        {
            Duration = duration;
            Category = category;
            SharedBudget = sharedBudget;
            Rows = new List<BudgetSummaryModelTableRow>();
            DurationDuration = duration.GetDurationDuration();
            StartDate = duration.GetStartDate();
            EndDate = duration.GetEndDate();
            TableId = (SharedBudget ? "Shared" : "") + Category.Name + "DataTable";
        }
    }

    public class BudgetSummaryModelTableRow
    {
        public string Name;
        public decimal Amount;
        public DateTime SpentDate;
        public decimal Percentage;
        public long ExpireDate;

        public BudgetSummaryModelTableRow(string store, decimal amount, DateTime spentDate, decimal percentage, long expireDate)
        {
            Name = store;
            Amount = amount;
            SpentDate = spentDate;
            Percentage = percentage;
            ExpireDate = expireDate;
        }
    }
}
