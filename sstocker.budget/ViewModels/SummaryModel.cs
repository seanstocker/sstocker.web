using Microsoft.AspNetCore.Mvc.Rendering;
using sstocker.budget.Enums;
using sstocker.budget.Models;
using sstocker.core.Helpers;
using System;
using System.Collections.Generic;

namespace sstocker.budget.ViewModels
{
    public class SummaryModel : BaseViewModel
    {
        public SummaryTimePeriod TimePeriod;
        public DateTime StartDate;
        public DateTime EndDate;
        public List<SummaryModelTable> Table;

        public SummaryModel(SummaryTimePeriod timePeriod, DateTime? minDate = null, DateTime? maxDate = null)
        {
            Table = new List<SummaryModelTable>();
            TimePeriod = timePeriod;

            switch (TimePeriod)
            {
                case SummaryTimePeriod.ThisWeek:
                    StartDate = DateTime.UtcNow.AddHours(-6).Date;
                    while (StartDate.DayOfWeek != DayOfWeek.Saturday)
                        StartDate = StartDate.AddDays(-1);
                    EndDate = StartDate.AddDays(7);
                    break;
                case SummaryTimePeriod.Last7Days:
                    StartDate = DateTime.UtcNow.AddHours(-6).Date.AddDays(-7);
                    EndDate = DateTime.UtcNow.AddHours(-6).Date;
                    break;
                case SummaryTimePeriod.ThisMonth:
                    StartDate = new DateTime(DateTime.UtcNow.AddHours(-6).Date.Year, DateTime.UtcNow.AddHours(-6).Date.Month, 1);
                    EndDate = StartDate.AddMonths(1).AddDays(-1);
                    break;
                case SummaryTimePeriod.LastMonth:
                    StartDate = new DateTime(DateTime.UtcNow.AddHours(-6).Date.Year, DateTime.UtcNow.AddHours(-6).Date.Month, 1).AddMonths(-1);
                    EndDate = StartDate.AddMonths(1).AddDays(-1);
                    break;
                case SummaryTimePeriod.Past30Days:
                    StartDate = DateTime.UtcNow.AddHours(-6).Date.AddDays(-30);
                    EndDate = DateTime.UtcNow.AddHours(-6).Date;
                    break;
                case SummaryTimePeriod.ThisYear:
                    StartDate = new DateTime(DateTime.UtcNow.AddHours(-6).Date.Year, 1, 1);
                    EndDate = StartDate.AddYears(1).AddDays(-1);
                    break;
                case SummaryTimePeriod.LastYear:
                    StartDate = new DateTime(DateTime.UtcNow.AddHours(-6).Date.Year, 1, 1).AddYears(-1);
                    EndDate = StartDate.AddYears(1).AddDays(-1);
                    break;
                case SummaryTimePeriod.AllTime:
                    StartDate = (minDate ?? DateTime.MinValue).Date;
                    EndDate = (maxDate ?? DateTime.MaxValue).Date;
                    break;
                default:
                    throw new Exception($"{TimePeriod} is not implemented yet.");
            }
        }

        public SelectList GetTimePeriodSelectList()
        {
            return TimePeriod.ToSelectList(TimePeriod);
        }
    }

    public class SummaryModelTable
    {
        public Category Category;
        public decimal Amount;
        public long Quantity;
        public decimal Percentage;
    }
}
