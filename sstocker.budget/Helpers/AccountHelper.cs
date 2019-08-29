using sstocker.budget.Repositories;

namespace sstocker.budget.Helpers
{
    public static class AccountHelper
    {
        public static bool HasSharedAccount(long accountId)
        {
            var sharedAccount = AccountRepository.GetSharedAccount(accountId);
            return sharedAccount != null;
        }

        public static long GetSharedAccountId(long accountId)
        {
            var sharedAccount = AccountRepository.GetSharedAccount(accountId);
            return sharedAccount.AccountId;
        }
    }
}
