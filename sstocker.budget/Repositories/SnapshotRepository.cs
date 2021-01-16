using sstocker.budget.Models;
using sstocker.core.Helpers;
using System;
using System.Collections.Generic;

namespace sstocker.budget.Repositories
{
    public static class SnapshotRepository
    {
        public static List<Snapshot> GetAccountSnapshots(long accountId)
        {
            var sql = @"
SELECT s.SnapShotId, s.AccountId, b.Name Bank, t.BankType BankType, s.Amount, s.SnapShotDate, eg.ExternalGuid
FROM Budget.dbo.SnapShot s
	JOIN Budget.dbo.Bank b
		ON s.BankId = b.BankId
	JOIN Budget.dbo.BankType t
		ON s.BankTypeId = t.BankTypeId
	JOIN Budget.dbo.ExternalGuid eg
		ON eg.ContextKey = 'SnapShotId'
		AND eg.ContextValue = s.SnapShotId
WHERE s.AccountId = @AccountId";

            var p = new
            {
                AccountId = accountId
            };

            var result = DatabaseHelper.Query<Snapshot>(sql, p);
            return result;
        }

        public static void AddSnapshot(long accountId, long bankId, long bankTypeId, decimal amount, DateTime date)
        {
            var sql = @"
DECLARE @SnapshotId TABLE (Id INT)

INSERT INTO Budget.dbo.Snapshot
OUTPUT INSERTED.SnapShotId INTO @SnapshotId
VALUES (@AccountId, @BankId, @BankTypeId, @Amount, @SnapShotDate)

INSERT INTO Budget.dbo.ExternalGuid
SELECT NEWID(), 'SnapShotId', Id
FROM @SnapshotId";

            var p = new
            {
                AccountId = accountId,
                BankId = bankId,
                BankTypeId = bankTypeId,
                Amount = amount,
                SnapShotDate = date
            };

            DatabaseHelper.Execute(sql, p);
        }
    }
}
