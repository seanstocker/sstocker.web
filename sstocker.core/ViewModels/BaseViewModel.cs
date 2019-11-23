using sstocker.core.Repositories;

namespace sstocker.core.ViewModels
{
    public class BaseViewModel
    {
        public string AccountName;
        public string AccountImage;

        public void SetBaseViewModel(long accountId)
        {
            var account = AccountRepository.GetAccount(accountId, true);
            AccountName = account.Name;
            AccountImage = account.Image;
        }
    }
}
