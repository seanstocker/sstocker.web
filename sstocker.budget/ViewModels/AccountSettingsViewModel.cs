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
        public AccountSettings<CategorySetting> Settings;
        public List<AccountSettingsViewModelCategory> Categories;
        public bool SharedAccountSettings;
        public bool HasSharedAccount;

        public AccountSettingsViewModel(AccountSettings<CategorySetting> settings, bool sharedAccountSettings, bool hasSharedAccount)
        {
            SharedAccountSettings = sharedAccountSettings;
            HasSharedAccount = hasSharedAccount;
            Settings = settings;
            Categories = new List<AccountSettingsViewModelCategory>();
            var allCategories = CategoryHelper.GetCategories();

            foreach (var category in allCategories)
                Categories.Add(new AccountSettingsViewModelCategory(category));

            foreach (var setting in Settings.Settings)
            {
                switch (setting.ContextKey.ToUpperInvariant())
                {
                    case SettingsHelper.CategorySettingKey:
                        Categories.Single(c => c.Category.CategoryId == long.Parse(setting.ContextValue)).Setting = setting.Data;
                        break;
                    default:
                        throw new Exception($"{setting.ContextKey} is not a valid settings categories.");
                }
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public string GetHtmlValues()
        {
            var categoryHtmlValues = new List<string>();

            foreach (var category in Categories)
                categoryHtmlValues.Add(category.GetHtmlValues());

            return string.Join(" + '|' + ", categoryHtmlValues);
        }
    }

    public class AccountSettingsViewModelCategory
    {
        public Category Category;
        public CategorySetting Setting;

        public AccountSettingsViewModelCategory(Category category)
        {
            Category = category;
            Setting = new CategorySetting();
        }

        public string HtmlRow()
        {
            var sb = new StringBuilder();

            var isActiveChecked = Setting.IsActive ? "checked=\"checked\"" : string.Empty;
            sb.AppendLine($"<input id=\"{Category.Name}_IsActive\" name=\"{Category.Name}_IsActive\" type=\"checkbox\" {isActiveChecked} onclick=\"onCheckboxChecked('{Category.Name}')\"/>");
            sb.AppendLine($"<label id=\"{Category.Name}_Label\" name=\"{Category.Name}_Label\" for=\"{Category.Name}_IsActive\">{Category.Name}</label>");

            sb.AppendLine("<br/>");

            var isActiveHidden = Setting.IsActive ? "style=\"display: block;\"" : "style=\"display: none;\"";
            sb.AppendLine($"<div id=\"{Category.Name}_Hidden_Values\" name=\"{Category.Name}_Hidden_Values\" {isActiveHidden}>");

            var isCriticalChecked = Setting.IsCritical ? "checked=\"checked\"" : string.Empty;
            sb.AppendLine($"<label id=\"{Category.Name}_IsCritical_Label\" name=\"{Category.Name}_IsCritical_Label\" for=\"{Category.Name}_IsCritical\" >Is Critical:</label>");
            sb.AppendLine($"<input id=\"{Category.Name}_IsCritical\" name=\"{Category.Name}_IsCritical\" type=\"checkbox\" {isCriticalChecked} />");

            sb.AppendLine("<br/>");

            var isUnlimitedChecked = Setting.Unlimited ? "checked=\"checked\"" : string.Empty;
            sb.AppendLine($"<label id=\"{Category.Name}_Unlimited_Label\" name=\"{Category.Name}_Unlimited_Label\" for=\"{Category.Name}_Unlimited\" >Unlimited Budget:</label>");
            sb.AppendLine($"<input id=\"{Category.Name}_Unlimited\" name=\"{Category.Name}_Unlimited\" type=\"checkbox\" {isUnlimitedChecked}  onclick=\"activateAmountBox('{Category.Name}')\"/>");

            sb.AppendLine("<br/>");

            var amountHidden = Setting.Unlimited ? "style=\"display: none;\"" : "style=\"display: block;\"";
            sb.AppendLine($"<div id=\"{Category.Name}_Amount_Hidden_Values\" name=\"{Category.Name}_Amount_Hidden_Values\" {amountHidden}>");
            sb.AppendLine($"<label id=\"{Category.Name}_Amount_Label\" name=\"{Category.Name}_Amount_Label\" for=\"{Category.Name}_Amount\" >Category Budget Amount:</label>");
            sb.AppendLine($"<input id=\"{Category.Name}_Amount\" name=\"{Category.Name}_Amount\" type=\"number\" min=\"1\" step=\"1\" value=\"{Setting.Amount}\" />");
            sb.AppendLine("</div>");

            sb.AppendLine($"<label id=\"{Category.Name}_Duration_Label\" name=\"{Category.Name}_Duration_Label\" for=\"{Category.Name}_Duration\" >Budget Duration:</label>");
            sb.AppendLine($"<select id=\"{Category.Name}_Duration\" name=\"{Category.Name}_Duration\">");
            foreach (Duration duration in Enum.GetValues(typeof(Duration)))
            {
                var selected = duration == Setting.Duration ? "selected" : string.Empty;
                sb.AppendLine($"<option value=\"{duration.ToString()}\" {selected}>{duration.ToString()}</option>");
            }
            sb.AppendLine("</select>");

            sb.AppendLine("</div>");

            return sb.ToString();
        }

        public string GetHtmlValues()
        {
            var sb = new StringBuilder();

            sb.Append($"'{Category.Name},' + ");
            sb.Append($"document.getElementById('{Category.Name}_IsActive').checked + ',' + ");
            sb.Append($"document.getElementById('{Category.Name}_IsCritical').checked + ',' + ");
            sb.Append($"document.getElementById('{Category.Name}_Unlimited').checked + ',' + ");
            sb.Append($"$('#{Category.Name}_Amount').val() + ',' + ");
            sb.Append($"$('#{Category.Name}_Duration').val()");

            return sb.ToString();
        }
    }
}
