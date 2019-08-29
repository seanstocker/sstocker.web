using sstocker.budget.Models;
using sstocker.core.Helpers;
using System.Collections.Generic;

namespace sstocker.budget.Repositories
{
    public static class IncomeSourceRepository
    {
        public static List<IncomeSource> GetAllIncomeSources()
        {
            var sql = @"
SELECT *
FROM Budget.dbo.IncomeSource";

            var result = DatabaseHelper.Query<IncomeSource>(sql);
            return result;
        }

        public static void CreateIncomeSource(string name)
        {
            var sql = @"
INSERT INTO Budget.dbo.IncomeSource
VALUES (@Name)";

            var p = new
            {
                Name = name
            };

            DatabaseHelper.Execute(sql, p);
        }
    }
}
