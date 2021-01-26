using Newtonsoft.Json;
using sstocker.budget.Enums;
using sstocker.budget.Helpers;
using sstocker.budget.Models;
using sstocker.core.Models;
using sstocker.core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sstocker.budget.ViewModels
{
    public class AccountSettingsViewModel : BaseViewModel
    {
        public List<AccountSettingsViewModelCategory> Categories;
        public AccountSettingsViewModelExpenseSummary ExpenseSummary;
        public bool SharedAccountSettings;
        public bool HasSharedAccount;

        public AccountSettingsViewModel(AccountSettings<CategorySetting> categorySettings, AccountSettings<ExpenseSummaryTimePeriod> expenseSummarySettings, bool sharedAccountSettings, bool hasSharedAccount)
        {
            SharedAccountSettings = sharedAccountSettings;
            HasSharedAccount = hasSharedAccount;
            Categories = SetupCategories(categorySettings);
            ExpenseSummary = SetupExpenseSummary(expenseSummarySettings);
        }

        private List<AccountSettingsViewModelCategory> SetupCategories(AccountSettings<CategorySetting> settings)
        {
            var categories = new List<AccountSettingsViewModelCategory>();
            var allCategories = CategoryHelper.GetCategories();

            foreach (var category in allCategories)
            {
                var setting = settings.Settings.SingleOrDefault(s => s.ContextKey == SettingsHelper.CategorySettingKey && long.Parse(s.ContextValue) == category.CategoryId);
                categories.Add(new AccountSettingsViewModelCategory(setting?.Data ?? new CategorySetting(), category.Name));
            }

            return categories;
        }

        private AccountSettingsViewModelExpenseSummary SetupExpenseSummary(AccountSettings<ExpenseSummaryTimePeriod> settings)
        {
            var timePeriod = SettingsHelper.GetTimePeriod(settings);
            return new AccountSettingsViewModelExpenseSummary(timePeriod ?? ExpenseSummaryTimePeriod.Default);
        }

        public string GetHtmlValues()
        {
            var htmlValues = new List<string>();

            foreach (var category in Categories)
                htmlValues.Add(category.GetHtmlValues());

            htmlValues.Add(ExpenseSummary.GetHtmlValues());

            return string.Join(" + '|' + ", htmlValues);
        }
    }

    public class AccountSettingViewModel<T>
    {
        public string Name;
        public string Type;
        public T Setting;

        public AccountSettingViewModel(T setting, string name, string type)
        {
            Setting = setting;
            Name = name;
            Type = type;
        }

        public virtual string GetHtmlValues()
        {
            throw new NotImplementedException();
        }
    }

    public class AccountSettingsViewModelCategory : AccountSettingViewModel<CategorySetting>
    {
        public AccountSettingsViewModelCategory(CategorySetting setting, string name)
            : base(setting, name, "CATEGORY")
        {
        }

        public override string GetHtmlValues()
        {
            var sb = new StringBuilder();

            sb.Append($"'--{Type}--' + ");
            sb.Append($"'{Name},' + ");
            sb.Append($"document.getElementById('{Name}_IsActive').checked + ',' + ");
            sb.Append($"document.getElementById('{Name}_IsCritical').checked + ',' + ");
            sb.Append($"document.getElementById('{Name}_Unlimited').checked + ',' + ");
            sb.Append($"$('#{Name}_Amount').val() + ',' + ");
            sb.Append($"document.querySelector('input[name=\"{Name}_Duration\"]:checked').value");

            return sb.ToString();
        }
    }

    public class AccountSettingsViewModelExpenseSummary : AccountSettingViewModel<ExpenseSummaryTimePeriod>
    {
        public AccountSettingsViewModelExpenseSummary(ExpenseSummaryTimePeriod setting)
            : base(setting, $"{SettingsHelper.ExpenseSummarySettingKey}_{SettingsHelper.TimePeriodSettingValue}", "EXPENSESUMMARYTIMEPERIOD")
        {
        }

        public override string GetHtmlValues()
        {
            return $"'--{Type}--' + document.querySelector('input[name=\"{Name}\"]:checked').value";
        }
    }
}
