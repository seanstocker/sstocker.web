using sstocker.budget.Helpers;
using sstocker.budget.Models;
using sstocker.budget.Repositories;
using sstocker.core.Helpers;
using sstocker.core.ViewModels;
using System;
using System.Collections.Generic;

namespace sstocker.budget.ViewModels
{
    public class EditExpenseModel : BaseViewModel
    {
        public Expense Expense;
        public List<string> Categories;
        public List<string> Stores;
        public bool HasSharedAccount;
        public bool IsSharedExpense;

        public EditExpenseModel(long accountId, Guid externalGuid)
        {
            Expense = ExpenseRepository.GetAccountExpense(externalGuid);
            Categories = CategoryHelper.GetCategoryNames(accountId);
            Stores = StoreHelper.GetStoreNames();
            HasSharedAccount = AccountHelper.HasSharedAccount(accountId);
            IsSharedExpense = Expense.AccountId != accountId;
        }
    }
}
