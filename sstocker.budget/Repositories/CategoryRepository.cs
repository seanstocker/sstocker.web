using sstocker.budget.Models;
using sstocker.core.Helpers;
using System.Collections.Generic;

namespace sstocker.budget.Repositories
{
    public static class CategoryRepository
    {
        public static List<Category> GetAllCategories()
        {
            var sql = @"
SELECT *
FROM Budget.dbo.Category";

            var result = DatabaseHelper.Query<Category>(sql);
            return result;
        }

        public static void CreateCategory(string name)
        {
            var sql = @"
INSERT INTO Budget.dbo.Category
VALUES (@Name)";

            var p = new
            {
                Name = name
            };

            DatabaseHelper.Execute(sql, p);
        }
    }
}
