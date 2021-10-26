using System;

namespace sstocker.budget.Models
{
    public class Snapshot
    {
        public long SnapShotId;
        public long AccountId;
        public string Bank;
        public string BankType;
        public decimal Amount;
        public bool IsDebt;
        public DateTime SnapShotDate;
        public Guid ExternalGuid;
    }
}
