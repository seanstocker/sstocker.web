using sstocker.budget.Models;
using sstocker.core.Helpers;
using System;
using System.Collections.Generic;

namespace sstocker.budget.Repositories
{
    public static class IncomeRepository
    {
        public static List<Income> GetAccountIncome(long accountId)
        {
            var sql = @"
SELECT i.IncomeId, i.AccountId, s.Name Source, t.Name Type, i.Amount, i.IncomeDate, eg.ExternalGuid
FROM Budget.dbo.Income i
	JOIN Budget.dbo.IncomeSource s
		ON i.IncomeSourceId = s.IncomeSourceId
	JOIN Budget.dbo.IncomeType t
		ON i.IncomeTypeId = t.IncomeTypeId
	JOIN Budget.dbo.ExternalGuid eg
		ON eg.ContextKey = 'IncomeId'
		AND eg.ContextValue = i.IncomeId
WHERE i.AccountId = @AccountId";

            var p = new
            {
                AccountId = accountId
            };

            var result = DatabaseHelper.Query<Income>(sql, p);
            return result;
        }

        public static List<Income> GetPartnerIncome(long accountId)
        {
            var partnerAccountId = SharedAccountRepository.GetPartnerAccountId(accountId);

            var sql = @"
SELECT i.IncomeId, i.AccountId, s.Name Source, t.Name Type, i.Amount, i.IncomeDate, eg.ExternalGuid
FROM Budget.dbo.Income i
	JOIN Budget.dbo.IncomeSource s
		ON i.IncomeSourceId = s.IncomeSourceId
	JOIN Budget.dbo.IncomeType t
		ON i.IncomeTypeId = t.IncomeTypeId
	JOIN Budget.dbo.ExternalGuid eg
		ON eg.ContextKey = 'IncomeId'
		AND eg.ContextValue = i.IncomeId
WHERE i.AccountId = @PartnerAccountId";

            var p = new
            {
                PartnerAccountId = partnerAccountId
            };

            var result = DatabaseHelper.Query<Income>(sql, p);
            return result;
        }

        public static void AddIncome(long accountId, long sourceId, long typeId, decimal amount, DateTime date)
        {
            var sql = @"
DECLARE @IncomeId TABLE (Id INT)

INSERT INTO Budget.dbo.Income
OUTPUT INSERTED.IncomeId INTO @IncomeId
VALUES (@AccountId, @SourceId, @TypeId, @Amount, @IncomeDate)

INSERT INTO Budget.dbo.ExternalGuid
SELECT NEWID(), 'IncomeId', Id
FROM @IncomeId";

            var p = new
            {
                AccountId = accountId,
                SourceId = sourceId,
                TypeId = typeId,
                Amount = amount,
                IncomeDate = date
            };

            DatabaseHelper.Execute(sql, p);
        }
    }
}
