using Newtonsoft.Json;
using sstocker.budget.Models;
using sstocker.core.Helpers;
using System;
using System.Linq;

namespace sstocker.budget.Repositories
{
    public static class AccountRepository
    {
        public static AccountWithPassword GetAccountWithPassword(string username)
        {
            var sql = @"
SELECT *
FROM Budget.dbo.Account
WHERE Username = @Username";

            var p = new
            {
                Username = username
            };

            var result = DatabaseHelper.QueryFirstOrDefault<AccountWithPassword>(sql, p);
            return result;
        }

        public static Account GetAccount(string username)
        {
            var sql = @"
SELECT *
FROM Budget.dbo.Account
WHERE Username = @Username";

            var p = new
            {
                Username = username
            };

            var result = DatabaseHelper.QueryFirstOrDefault<Account>(sql, p);
            return result;
        }

        public static Account GetAccount(long accountId)
        {
            var sql = @"
SELECT *
FROM Budget.dbo.Account
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
INSERT INTO Budget.dbo.Account
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
FROM Budget.dbo.AccountSettings s
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
IF EXISTS (SELECT * FROM Budget.dbo.AccountSettings WHERE AccountId = @Id AND ContextKey = @ContextKey AND ContextValue = @ContextValue)
BEGIN

UPDATE Budget.dbo.AccountSettings
SET Data = @Data
WHERE AccountId = @Id
	AND ContextKey = @ContextKey
	AND ContextValue = @ContextValue

END
ELSE
BEGIN

INSERT INTO Budget.dbo.AccountSettings
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

        public static Account GetSharedAccount(long accountId)
        {
            var accountSettings = GetAccountSettings<dynamic>(accountId, "ACCOUNTSHARINGACCOUNTID");
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
    }
}
