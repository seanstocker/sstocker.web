using sstocker.budget.Helpers;
using sstocker.core.ViewModels;
using System.Collections.Generic;

namespace sstocker.budget.ViewModels
{
    public class AddIncomeModel : BaseViewModel
    {
        public List<string> IncomeSources;
        public List<string> Types;

        public AddIncomeModel(long accountId)
        {
            IncomeSources = IncomeSourceHelper.GetIncomeSourceNames(accountId);
            Types = IncomeTypeHelper.GetIncomeTypeNames();
        }
    }
}
