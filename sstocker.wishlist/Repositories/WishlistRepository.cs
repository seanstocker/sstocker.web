using sstocker.core.Helpers;
using sstocker.wishlist.Models;
using System.Collections.Generic;

namespace sstocker.wishlist.Repositories
{
    public static class WishlistRepository
    {
        public static List<Wishlist> GetWishlist(long accountId)
        {
            var sql = @"
SELECT *
FROM Wishlist.dbo.Wishlist
WHERE AccountId = @AccountId";

            var p = new
            {
                AccountId = accountId
            };

            var result = DatabaseHelper.Query<Wishlist>(sql, p);
            return result;
        }

        public static List<long> GetSharedAccountIds(long accountId)
        {
            var sql = @"
SELECT SharedAccountsId
FROM Wishlist.dbo.SharedAccounts
WHERE PrimaryAccountId = @AccountId";

            var p = new
            {
                AccountId = accountId
            };

            var result = DatabaseHelper.Query<long>(sql, p);
            return result;
        }
    }
}
