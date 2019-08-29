using System;

namespace sstocker.budget.Models
{
    public class Expense
    {
        public long ExpenseId;
        public string Store;
        public string Category;
        public decimal Amount;
        public DateTime SpentDate;
        public Guid ExternalGuid;
        public long AccountId;
        public long SpentAccountId;
    }
}
