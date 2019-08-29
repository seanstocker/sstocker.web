using sstocker.budget.Models;
using sstocker.core.Helpers;
using System.Collections.Generic;

namespace sstocker.budget.Repositories
{
    public static class IncomeTypeRepository
    {
        public static List<IncomeType> GetAllIncomeTypes()
        {
            var sql = @"
SELECT *
FROM Budget.dbo.IncomeType";

            var result = DatabaseHelper.Query<IncomeType>(sql);
            return result;
        }

        public static void CreateIncomeType(string name)
        {
            var sql = @"
INSERT INTO Budget.dbo.IncomeType
VALUES (@Name)";

            var p = new
            {
                Name = name
            };

            DatabaseHelper.Execute(sql, p);
        }
    }
}
