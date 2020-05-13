using sstocker.core.ViewModels;
using System;
using System.Collections.Generic;

namespace sstocker.budget.ViewModels
{
    public class DashboardModel : BaseViewModel
    {
        public decimal SpentToday;
        public decimal SpentThisWeek;
        public decimal SpentThisMonth;
        public decimal SpentThisYear;
        public ChartJSChartModel ExpensesLineChart;
        public ChartJSChartModel IncomeLineChart;
        public ChartJSChartModel SharedExpensesLineChart;
        public ChartJSChartModel SpentExpensesLineChart;
        public ChartJSChartModel PieChart;
        public List<BudgetOverview> BudgetOverviewList;
        public SharedDashboardModel Shared;

        public DashboardModel()
        {
            ExpensesLineChart = new ChartJSChartModel();
            IncomeLineChart = new ChartJSChartModel();
            SharedExpensesLineChart = new ChartJSChartModel();
            SpentExpensesLineChart = new ChartJSChartModel();
            PieChart = new ChartJSChartModel();
            BudgetOverviewList = new List<BudgetOverview>();
            Shared = new SharedDashboardModel();
        }

        public string GetPrimaryColor(int location)
        {
            return GetPrimaryColors()[location];
        }

        public string GetSecondaryColor(int location)
        {
            return GetSecondaryColors()[location];
        }

        public List<string> GetPrimaryColors(int? count = null)
        {
            var colors = new List<string> { "#4e73df", "#da5367", "#d5589b", "#e3ff2e", "#2ef2ff", "#2effbf", "#5bd264", "#e3ff2e", "#2ea8ff", "#af68c5", "#5bd264", "#ffbf2e", "#ffea2e", "#2ebdff", "#ad55d8", "#a3ff2e", "#ffae2e" };
            if (count.HasValue)
                colors.RemoveRange(count.GetValueOrDefault(), colors.Count - count.GetValueOrDefault());
            return colors;
        }

        public List<string> GetSecondaryColors(int? count = null)
        {
            var colors = new List<string> { "#2e59d9", "#ff0848", "#ff0895", "#aaff08", "#08d6ff", "#08ffb0", "#ff5f08", "#aaff08", "#0881ff", "#ba3dca", "#08ff16", "#ffb508", "#ffe708", "#08a3ff", "#b721e6", "#5eff08", "#ff9408" };
            if (count.HasValue)
                colors.RemoveRange(count.GetValueOrDefault(), colors.Count - count.GetValueOrDefault());
            return colors;
        }
    }

    public class SharedDashboardModel
    {
        public bool HasSharedAccount;
        public bool SharedDashboard;
        public string PartnerName;
        public List<SharedOweAmount> OweAmount;

        public SharedDashboardModel()
        {
            OweAmount = new List<SharedOweAmount>();
        }

        public SharedDashboardModel(bool hasSharedAccount, bool sharedDashboard, string partnerName)
        {
            HasSharedAccount = hasSharedAccount;
            SharedDashboard = sharedDashboard;
            PartnerName = partnerName;
            OweAmount = new List<SharedOweAmount>();
        }

        public void AddSharedOweAmount(decimal oweAmount, DateTime month)
        {
            OweAmount.Add(new SharedOweAmount(oweAmount, month));
        }
    }

    public class SharedOweAmount
    {
        public decimal OweAmount;
        public DateTime Month;

        public SharedOweAmount(decimal oweAmount, DateTime month)
        {
            OweAmount = oweAmount;
            Month = month;
        }
    }

    public class ChartJSChartModel
    {
        public List<string> Labels;
        public List<decimal> Amounts;
        public bool Show;

        public ChartJSChartModel()
        {
            Labels = new List<string>();
            Amounts = new List<decimal>();
            Show = true;
        }

        public void AddPoint(string label, decimal amount)
        {
            Labels.Add(label);
            Amounts.Add(amount);
        }

        public void AddPointBeginning(string label, decimal amount)
        {
            Labels.Insert(0, label);
            Amounts.Insert(0, amount);
        }

        public int GetCount()
        {
            if (Labels.Count != Amounts.Count)
                throw new System.Exception($"Do not have the same amount of Labels and Amounts.");
            return Labels.Count;
        }
    }

    public class BudgetOverview
    {
        public string Name;
        public string Duration;
        public decimal SpentTotal;
        public decimal RemainingTotal;
        public decimal SettingsTotal;
        public int PercentageUsed;

        public BudgetOverview(string name, string duration, decimal spentTotal, decimal settingsTotal)
        {
            Name = name;
            Duration = duration;
            SpentTotal = spentTotal;
            RemainingTotal = settingsTotal - spentTotal;
            SettingsTotal = settingsTotal;
            PercentageUsed = (int)Math.Round(spentTotal / settingsTotal * 100);
        }

        public string GetPercentageClassName()
        {
            if (PercentageUsed < 20)
                return "bg-success";
            if (PercentageUsed < 40)
                return "bg-info";
            if (PercentageUsed < 60)
                return "";
            if (PercentageUsed < 80)
                return "bg-warning";
            return "bg-danger";

        }
    }
}
