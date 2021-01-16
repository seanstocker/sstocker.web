using sstocker.budget.Helpers;
using sstocker.core.ViewModels;
using System.Collections.Generic;

namespace sstocker.budget.ViewModels
{
    public class AddSnapshotModel : BaseViewModel
    {
        public List<string> Banks;
        public List<string> BankTypes;

        public AddSnapshotModel()
        {
            Banks = BankHelper.GetBankNames();
            BankTypes = BankHelper.GetBankTypeNames();
        }
    }
}