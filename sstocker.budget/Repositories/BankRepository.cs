using sstocker.budget.Models;
using sstocker.core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace sstocker.budget.Repositories
{
    public static class BankRepository
    {
        public static List<Bank> GetAllBanks()
        {
            var sql = @"
SELECT *
FROM Budget.dbo.Bank";

            var result = DatabaseHelper.Query<Bank>(sql);
            return result;
        }

        public static void CreateBank(string name)
        {
            var sql = @"
INSERT INTO Budget.dbo.Bank
VALUES (@Name)";

            var p = new
            {
                Name = name
            };

            DatabaseHelper.Execute(sql, p);
        }

        public static List<BankTypeModel> GetAllBankTypes()
        {
            var sql = @"
SELECT *
FROM Budget.dbo.BankType";

            var result = DatabaseHelper.Query<BankTypeModel>(sql);
            return result;
        }

        public static void CreateBankType(string bankType)
        {
            var sql = @"
INSERT INTO Budget.dbo.BankType
VALUES (@BankType)";

            var p = new
            {
                BankType = bankType
            };

            DatabaseHelper.Execute(sql, p);
        }
    }
}
