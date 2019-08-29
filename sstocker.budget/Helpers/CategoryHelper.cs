using sstocker.budget.Models;
using sstocker.budget.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace sstocker.budget.Helpers
{
    public static class CategoryHelper
    {
        private static List<Category> Categories;

        static CategoryHelper()
        {
            Categories = CategoryRepository.GetAllCategories();
        }

        public static List<Category> GetCategories(long accountId = 0)
        {
            if (Categories == null || !Categories.Any())
                Categories = CategoryRepository.GetAllCategories();

            if (accountId <= 0)
                return Categories;

            var settings = AccountRepository.GetAccountSettings<CategorySetting>(accountId, SettingsHelper.CategorySettingKey);
            var returnCategories = Categories.ToList();
            foreach (var setting in settings.Settings)
            {
                if (!setting.Data.IsActive)
                    returnCategories.RemoveAll(c => c.CategoryId == setting.ContextValue);
            }

            return returnCategories;
        }

        public static List<string> GetCategoryNames(long accountId = 0)
        {
            return GetCategories(accountId).Select(c => c.Name).ToList();
        }

        public static long GetCategoryId(string name)
        {
            return Categories.Single(c => c.Name == name).CategoryId;
        }

        public static void AddCategory(string name)
        {
            if (Categories.Any(c => c.Name == name))
                return;

            CategoryRepository.CreateCategory(name);
            Categories = CategoryRepository.GetAllCategories();
        }
    }
}
