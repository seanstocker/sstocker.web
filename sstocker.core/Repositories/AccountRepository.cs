using Newtonsoft.Json;
using sstocker.budget.Models;
using sstocker.core.Helpers;
using System;

namespace sstocker.budget.Repositories
{
    public static class AccountRepository
    {
        public static AccountWithPassword GetAccountWithPassword(string username)
        {
            var sql = @"
SELECT *
FROM Account.dbo.Account
WHERE Username = @Username";

            var p = new
            {
                Username = username
            };

            var result = DatabaseHelper.QueryFirstOrDefault<AccountWithPassword>(sql, p);
            return result;
        }

        public static Account GetAccount(string username, bool getImage = false)
        {
            var sql = getImage
                ? @"
SELECT a.*, ai.Image
FROM Account.dbo.Account a
	LEFT JOIN Account.dbo.AccountImage ai
		ON a.AccountId = ai.AccountId
WHERE a.Username = @Username"
                : @"
SELECT *
FROM Account.dbo.Account
WHERE Username = @Username";

            var p = new
            {
                Username = username
            };

            var result = DatabaseHelper.QueryFirstOrDefault<Account>(sql, p);
            return result;
        }

        public static Account GetAccount(long accountId, bool getImage = false)
        {
            var sql = getImage
                ? @"
SELECT a.*, ai.Image
FROM Account.dbo.Account a
	LEFT JOIN Account.dbo.AccountImage ai
		ON a.AccountId = ai.AccountId
WHERE a.AccountId = @AccountId"
                : @"
SELECT *
FROM Account.dbo.Account
WHERE AccountId = @AccountId";

            var p = new
            {
                AccountId = accountId
            };

            var result = DatabaseHelper.QueryFirstOrDefault<Account>(sql, p);
            return result;
        }

        public static void CreateAccount(string username, string password, string salt, string name)
        {
            var sql = @"
INSERT INTO Account.dbo.Account
VALUES (@Username, @Password, @Salt, @Name, @Date)";

            var p = new
            {
                Username = username,
                Password = password,
                Salt = salt,
                Name = name,
                Date = DateTime.UtcNow
            };

            DatabaseHelper.Execute(sql, p);
        }

        public static AccountSettings<T> GetAccountSettings<T>(long accountId, string contextKey)
        {
            var sql = @"
SELECT s.ContextKey, s.ContextValue, s.Data
FROM Account.dbo.AccountSettings s
WHERE s.AccountId = @Id
    AND s.ContextKey = @Key";

            var p = new
            {
                Id = accountId,
                Key = contextKey
            };

            var result = DatabaseHelper.Query<AccountSetting<string>>(sql, p);
            return new AccountSettings<T>(accountId, result);
        }

        public static void AddOrUpdateAccountSetting(long accountId, string contextKey, long contextValue, object data)
        {
            var sql = @"
IF EXISTS (SELECT * FROM Account.dbo.AccountSettings WHERE AccountId = @Id AND ContextKey = @ContextKey AND ContextValue = @ContextValue)
BEGIN

UPDATE Account.dbo.AccountSettings
SET Data = @Data
WHERE AccountId = @Id
	AND ContextKey = @ContextKey
	AND ContextValue = @ContextValue

END
ELSE
BEGIN

INSERT INTO Account.dbo.AccountSettings
VALUES (@Id, @ContextKey, @ContextValue, @Data)

END";

            var p = new
            {
                Id = accountId,
                ContextKey = contextKey,
                ContextValue = contextValue,
                Data = JsonConvert.SerializeObject(data)
            };

            DatabaseHelper.Execute(sql, p);
        }

        public static void DeleteAccountImage(long accountId)
        {
            var sql = @"
DELETE FROM Account.dbo.AccountImage
WHERE AccountId = @AccountId";

            var p = new
            {
                AccountId = accountId
            };

            DatabaseHelper.Execute(sql, p);
        }

        public static void UpdateAccountImage(long accountId, string accountImage)
        {
            var sql = @"
IF EXISTS (SELECT * FROM Account.dbo.AccountImage WHERE AccountId = @AccountId)
BEGIN

	UPDATE Account.dbo.AccountImage
	SET Image = @AccountImage
	WHERE AccountId = @AccountId

END
ELSE
BEGIN

	INSERT INTO Account.dbo.AccountImage
	VALUES (@AccountId, @AccountImage)

END";

            var p = new
            {
                AccountId = accountId,
                AccountImage = accountImage
            };

            DatabaseHelper.Execute(sql, p);
        }

        public static void UpdateAccountImage(long accountId, object image)
        {
            throw new NotImplementedException();
        }
    }
}
