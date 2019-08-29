using sstocker.budget.Models;
using sstocker.core.Helpers;
using System;
using System.Collections.Generic;

namespace sstocker.budget.Repositories
{
    public static class ExpenseRepository
    {
        public static List<Expense> GetAccountExpenses(long accountId)
        {
            var sql = @"
SELECT e.ExpenseId, e.AccountId, s.Name Store, c.Name Category, e.Amount, e.SpentDate, eg.ExternalGuid, e.SpentAccountId
FROM Budget.dbo.Expense e
	JOIN Budget.dbo.Store s
		ON e.StoreId = s.StoreId
	JOIN Budget.dbo.Category c
		ON e.CategoryId = c.CategoryId
	JOIN Budget.dbo.ExternalGuid eg
		ON eg.ContextKey = 'ExpenseId'
		AND eg.ContextValue = e.ExpenseId
WHERE e.AccountId = @AccountId";

            var p = new
            {
                AccountId = accountId
            };

            var result = DatabaseHelper.Query<Expense>(sql, p);
            return result;
        }

        public static List<Expense> GetAccountExpensesIncludeShared(long accountId)
        {
            var sql = @"
SELECT e.ExpenseId, e.AccountId, s.Name Store, c.Name Category, e.Amount, e.SpentDate, eg.ExternalGuid, e.SpentAccountId
FROM Budget.dbo.Expense e
	JOIN Budget.dbo.Store s
		ON e.StoreId = s.StoreId
	JOIN Budget.dbo.Category c
		ON e.CategoryId = c.CategoryId
	JOIN Budget.dbo.ExternalGuid eg
		ON eg.ContextKey = 'ExpenseId'
		AND eg.ContextValue = e.ExpenseId
WHERE e.SpentAccountId = @AccountId";

            var p = new
            {
                AccountId = accountId
            };

            var result = DatabaseHelper.Query<Expense>(sql, p);
            return result;
        }

        public static Expense GetAccountExpense(Guid externalGuid)
        {
            var sql = @"
SELECT e.ExpenseId, s.Name Store, c.Name Category, e.Amount, e.SpentDate, eg.ExternalGuid, e.AccountId, e.SpentAccountId
FROM Budget.dbo.Expense e
	JOIN Budget.dbo.Store s
		ON e.StoreId = s.StoreId
	JOIN Budget.dbo.Category c
		ON e.CategoryId = c.CategoryId
	JOIN Budget.dbo.ExternalGuid eg
		ON eg.ContextKey = 'ExpenseId'
		AND eg.ContextValue = e.ExpenseId
WHERE eg.ExternalGuid = @Guid";

            var p = new
            {
                Guid = externalGuid
            };

            var result = DatabaseHelper.QueryFirstOrDefault<Expense>(sql, p);
            return result;
        }

        public static void AddExpense(long accountId, long storeId, long categoryId, decimal amount, DateTime date, long spentAccountId)
        {
            var sql = @"
DECLARE @ExpenseId TABLE (Id INT)

INSERT INTO Budget.dbo.Expense
OUTPUT INSERTED.ExpenseId INTO @ExpenseId
VALUES (@AccountId, @StoreId, @CategoryId, @Amount, @SpentDate, @SpentAccountId)

INSERT INTO Budget.dbo.ExternalGuid
SELECT NEWID(), 'ExpenseId', Id
FROM @ExpenseId";

            var p = new
            {
                AccountId = accountId,
                StoreId = storeId,
                CategoryId = categoryId,
                Amount = amount,
                SpentDate = date,
                SpentAccountId = spentAccountId
            };

            DatabaseHelper.Execute(sql, p);
        }

        public static void EditExpense(Guid externalGuid, long accountId, long storeId, long categoryId, decimal amount, DateTime date)
        {
            var sql = @"
UPDATE e
SET AccountId = @AccountId,
	StoreId = @StoreId,
	CategoryId = @CategoryId,
	Amount = @Amount,
	SpentDate = @SpentDate
FROM Budget.dbo.Expense e
	JOIN Budget.dbo.ExternalGuid eg
		on eg.ContextKey = 'ExpenseId'
		and eg.ContextValue = e.ExpenseId
WHERE eg.ExternalGuid = @Guid";

            var p = new
            {
                Guid = externalGuid,
                AccountId = accountId,
                StoreId = storeId,
                CategoryId = categoryId,
                Amount = amount,
                SpentDate = date
            };

            DatabaseHelper.Execute(sql, p);
        }

        public static void DeleteExpense(Guid externalGuid)
        {
            var sql = @"
DECLARE @ExpenseId INT = 
	(SELECT e.ExpenseId
	FROM Budget.dbo.Expense e
		JOIN Budget.dbo.ExternalGuid eg
			on eg.ContextKey = 'ExpenseId'
			and eg.ContextValue = e.ExpenseId
	WHERE eg.ExternalGuid = @Guid)

DELETE FROM Budget.dbo.ExternalGuid
WHERE ContextKey = 'ExpenseId'
	AND ContextValue = @ExpenseId

DELETE FROM Budget.dbo.Expense
WHERE ExpenseId = @ExpenseId";

            var p = new
            {
                Guid = externalGuid,
            };

            DatabaseHelper.Execute(sql, p);
        }
    }
}
