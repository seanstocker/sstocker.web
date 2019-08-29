using System;

namespace sstocker.budget.Enums
{
    public enum ExpenseSummaryTimePeriod
    {
        Today,
        ThisWeek,
        Last7Days,
        ThisMonth,
        LastMonth,
        Past30Days,
        ThisYear,
        LastYear,
        AllTime
    }

    public static class ExpenseSummaryTimePeriodHelper
    {
        public static DateTime GetStartDate(this ExpenseSummaryTimePeriod timePeriod, DateTime? minDate = null)
        {
            var date = DateTime.UtcNow.AddHours(-6).Date;

            switch (timePeriod)
            {
                case ExpenseSummaryTimePeriod.Today:
                    break;
                case ExpenseSummaryTimePeriod.ThisWeek:
                    while (date.DayOfWeek != DayOfWeek.Saturday)
                        date = date.AddDays(-1);
                    break;
                case ExpenseSummaryTimePeriod.Last7Days:
                    date = date.AddDays(-7);
                    break;
                case ExpenseSummaryTimePeriod.ThisMonth:
                    date = new DateTime(date.Date.Year, date.Month, 1);
                    break;
                case ExpenseSummaryTimePeriod.LastMonth:
                    date = new DateTime(date.Year, date.Month, 1).AddMonths(-1);
                    break;
                case ExpenseSummaryTimePeriod.Past30Days:
                    date = date.AddDays(-30);
                    break;
                case ExpenseSummaryTimePeriod.ThisYear:
                    date = new DateTime(date.Year, 1, 1);
                    break;
                case ExpenseSummaryTimePeriod.LastYear:
                    date = new DateTime(date.Year, 1, 1).AddYears(-1);
                    break;
                case ExpenseSummaryTimePeriod.AllTime:
                    date = (minDate ?? DateTime.MinValue).Date;
                    break;
                default:
                    throw new Exception($"{timePeriod} is not implemented yet.");
            }

            return date;
        }

        public static DateTime GetEndDate(this ExpenseSummaryTimePeriod timePeriod, DateTime? maxDate = null)
        {
            var date = DateTime.UtcNow.AddHours(-6).Date;

            switch (timePeriod)
            {
                case ExpenseSummaryTimePeriod.Today:
                    break;
                case ExpenseSummaryTimePeriod.ThisWeek:
                    while (date.DayOfWeek != DayOfWeek.Saturday)
                        date = date.AddDays(-1);
                    date = date.AddDays(7);
                    break;
                case ExpenseSummaryTimePeriod.Last7Days:
                    break;
                case ExpenseSummaryTimePeriod.ThisMonth:
                    date = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
                    break;
                case ExpenseSummaryTimePeriod.LastMonth:
                    date = new DateTime(date.Year, date.Month, 1).AddDays(-1);
                    break;
                case ExpenseSummaryTimePeriod.Past30Days:
                    break;
                case ExpenseSummaryTimePeriod.ThisYear:
                    date = new DateTime(date.Year, 1, 1).AddYears(1).AddDays(-1);
                    break;
                case ExpenseSummaryTimePeriod.LastYear:
                    date = new DateTime(date.Year, 1, 1).AddDays(-1);
                    break;
                case ExpenseSummaryTimePeriod.AllTime:
                    date = (maxDate ?? DateTime.MaxValue).Date;
                    break;
                default:
                    throw new Exception($"{timePeriod} is not implemented yet.");
            }

            return date;
        }
    }
}
