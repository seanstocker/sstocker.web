using System;

namespace sstocker.budget.Models
{
    public class SharedAccountMoneyTransfer
    {
        public long SharedAccountMoneyTransferId;
        public long SharedAccountId;
        public long PayerAccountId;
        public long PayedAccountId;
        public int ForMonth;
        public int ForYear;
        public decimal Amount;
        public DateTime TransferDate;
        public Guid ExternalGuid;
    }
}
