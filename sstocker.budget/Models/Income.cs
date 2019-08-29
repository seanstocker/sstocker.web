using System;

namespace sstocker.budget.Models
{
    public class Income
    {
        public long IncomeId;
        public long AccountId;
        public string Source;
        public string Type;
        public decimal Amount;
        public DateTime IncomeDate;
        public Guid ExternalGuid;
    }
}
