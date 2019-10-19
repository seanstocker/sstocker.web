using sstocker.budget.Models;
using sstocker.core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sstocker.budget.Repositories
{
    public static class SharedAccountRepository
    {
        public static Account GetSharedAccount(long accountId)
        {
            var accountSettings = AccountRepository.GetAccountSettings<dynamic>(accountId, "ACCOUNTSHARINGACCOUNTID");
            if (accountSettings.Settings.Any())
            {
                var sharedAccountSql = @"
SELECT *
FROM Budget.dbo.Account
WHERE AccountId = @AccountId";

                var sharedAccountParameters = new
                {
                    AccountId = accountSettings.Settings.FirstOrDefault().ContextValue
                };

                return DatabaseHelper.QueryFirstOrDefault<Account>(sharedAccountSql, sharedAccountParameters);
            }
            return null;
        }

        public static string GetPartnerName(long accountId)
        {
            var sql = @"
SELECT a.Name
FROM Budget.dbo.AccountSettings s
	JOIN Budget.dbo.AccountSettings s2
		ON s.AccountSettingsId != s2.AccountSettingsId
			AND s.ContextKey = 'ACCOUNTSHARINGACCOUNTID'
			AND s2.ContextKey = 'ACCOUNTSHARINGACCOUNTID'
			AND s.ContextValue = s2.ContextValue
	JOIN Budget.dbo.Account a
		ON a.AccountId = s2.AccountId
WHERE s.AccountId = @AccountId";

            var p = new
            {
                AccountId = accountId
            };

            var result = DatabaseHelper.QueryFirstOrDefault<string>(sql, p);
            return result;
        }

        public static List<SharedAccountMoneyTransfer> GetSharedAccountMoneyTransfer(long accountId)
        {
            var sql = @"
SELECT samt.SharedAccountMoneyTransferId, samt.SharedAccountId, samt.PayerAccountId, samt.PayedAccountId, samt.Amount, samt.TransferDate, samt.ForMonth, samt.ForYear, eg.ExternalGuid
FROM Budget.dbo.SharedAccountMoneyTransfer samt
	JOIN Budget.dbo.ExternalGuid eg
		ON eg.ContextKey = 'SharedAccountMoneyTransferId'
		AND eg.ContextValue = samt.SharedAccountMoneyTransferId
WHERE samt.SharedAccountId = @AccountId";

            var p = new
            {
                AccountId = accountId
            };

            var result = DatabaseHelper.Query<SharedAccountMoneyTransfer>(sql, p);
            return result;
        }

        public static void AddSharedAccountMoneyTransfer(long sharedAccountId, long payedAccountId, decimal amount, DateTime transferDate, int forMonth, int forYear)
        {
            var payerAccountId = GetPartnerAccountId(payedAccountId);

            var sql = @"
DECLARE @SharedAccountMoneyTransferId TABLE (Id INT)

INSERT INTO Budget.dbo.SharedAccountMoneyTransfer
OUTPUT INSERTED.SharedAccountMoneyTransferId INTO @SharedAccountMoneyTransferId
VALUES (@SharedAccountId, @PayerAccountId, @PayedAccountId, @Amount, @TransferDate, @ForMonth, @ForYear)

INSERT INTO Budget.dbo.ExternalGuid
SELECT NEWID(), 'SharedAccountMoneyTransferId', Id
FROM @SharedAccountMoneyTransferId";

            var p = new
            {
                SharedAccountId = sharedAccountId,
                PayerAccountId = payerAccountId,
                PayedAccountId = payedAccountId,
                Amount = amount,
                TransferDate = transferDate,
                ForMonth = forMonth,
                ForYear = forYear
            };

            DatabaseHelper.Execute(sql, p);
        }

        private static long GetPartnerAccountId(long accountId)
        {
            var sql = @"
SELECT a.AccountId
FROM Budget.dbo.AccountSettings s
	JOIN Budget.dbo.AccountSettings s2
		ON s.AccountSettingsId != s2.AccountSettingsId
			AND s.ContextKey = 'ACCOUNTSHARINGACCOUNTID'
			AND s2.ContextKey = 'ACCOUNTSHARINGACCOUNTID'
			AND s.ContextValue = s2.ContextValue
	JOIN Budget.dbo.Account a
		ON a.AccountId = s2.AccountId
WHERE s.AccountId = @AccountId";

            var p = new
            {
                AccountId = accountId
            };

            var result = DatabaseHelper.QueryFirstOrDefault<long>(sql, p);
            return result;
        }
    }
}