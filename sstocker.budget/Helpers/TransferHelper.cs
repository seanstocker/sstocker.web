using sstocker.budget.Repositories;
using System;

namespace sstocker.budget.Helpers
{
    public static class TransferHelper
    {
        public static void TransferMoney(long accountId, decimal amount, DateTime forDate)
        {
            TransferMoney(accountId, amount, forDate.Month, forDate.Year);
        }

        public static void TransferMoney(long accountId, decimal amount, int forMonth, int forYear)
        {
            if (!AccountHelper.HasSharedAccount(accountId))
                throw new Exception($"{accountId} does not have a shared account to transfer money with.");

            var sharedAccountId = AccountHelper.GetSharedAccountId(accountId);
            var today = DateTime.UtcNow.AddHours(-6).Date;

            SharedAccountRepository.AddSharedAccountMoneyTransfer(sharedAccountId, accountId, amount, today, forMonth, forYear);
        }
    }
}
