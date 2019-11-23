using sstocker.core.Helpers;
using sstocker.core.Repositories;
using System;

namespace sstocker.core.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public string CurrentProfileImage;
        public bool IsDefaultImage;
        public string Layout;
        public string Site;

        public ProfileViewModel(long accountId, string site)
        {
            var account = AccountRepository.GetAccount(accountId, true);
            CurrentProfileImage = account.Image;
            IsDefaultImage = account.IsDefaultImage();
            Site = site;

            switch (site)
            {
                case LoginHelper.BudgetApp:
                    Layout = "_budgetLayout";
                    break;
                case LoginHelper.WishlistApp:
                    Layout = "_wishlistLayout";
                    break;
                default:
                    throw new Exception($"{site} is not a valid site.");
            }
        }
    }
}