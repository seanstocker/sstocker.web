using sstocker.core.ViewModels;

namespace sstocker.wishlist.ViewModels
{
    public class AddWishlistItemViewModel : BaseViewModel
    {
        public long AccountId;

        public AddWishlistItemViewModel(long accountId)
        {
            AccountId = accountId;
            SetBaseViewModel(accountId);
        }
    }
}