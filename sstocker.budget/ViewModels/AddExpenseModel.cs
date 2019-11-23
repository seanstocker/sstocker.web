using sstocker.budget.Helpers;
using sstocker.core.Helpers;
using sstocker.core.ViewModels;
using System.Collections.Generic;

namespace sstocker.budget.ViewModels
{
    public class AddExpenseModel : BaseViewModel
    {
        public List<string> Categories;
        public List<string> Stores;
        public bool HasSharedAccount;

        public AddExpenseModel(long accountId)
        {
            HasSharedAccount = AccountHelper.HasSharedAccount(accountId);
            Categories = CategoryHelper.GetCategoryNames(accountId);
            Stores = StoreHelper.GetStoreNames();
        }
    }
}
