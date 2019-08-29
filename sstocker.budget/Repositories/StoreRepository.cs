using sstocker.budget.Models;
using sstocker.core.Helpers;
using System.Collections.Generic;

namespace sstocker.budget.Repositories
{
    public static class StoreRepository
    {
        public static List<Store> GetAllStores()
        {
            var sql = @"
SELECT *
FROM Budget.dbo.Store";

            var result = DatabaseHelper.Query<Store>(sql);
            return result;
        }

        public static void CreateStore(string name)
        {
            var sql = @"
INSERT INTO Budget.dbo.Store
VALUES (@Name)";

            var p = new
            {
                Name = name
            };

            DatabaseHelper.Execute(sql, p);
        }
    }
}
