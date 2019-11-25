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
WHERE AccountId = @AccountId
    AND IsActive = 1";

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
SELECT SharingAccountId
FROM Wishlist.dbo.SharedAccounts
WHERE PrimaryAccountId = @AccountId";

            var p = new
            {
                AccountId = accountId
            };

            var result = DatabaseHelper.Query<long>(sql, p);
            return result;
        }

        public static void AddWishlistItem(long accountId, string name, string description, string link)
        {
            var sql = @"
INSERT INTO Wishlist.dbo.Wishlist
VALUES (@AccountId, @Name, @Link, @Description, GETDATE(), 1, 0)";

            var p = new
            {
                AccountId = accountId,
                Name = name,
                Description = description,
                Link = link
            };

            DatabaseHelper.Execute(sql, p);
        }

        public static bool WishlistItemExists(long accountId, string name)
        {
            var sql = @"
SELECT COUNT(*)
FROM Wishlist.dbo.Wishlist
WHERE AccountId = @AccountId
	AND Name = @Name
	AND IsActive = 1";

            var p = new
            {
                AccountId = accountId,
                Name = name
            };

            var result = DatabaseHelper.QueryFirstOrDefault<long>(sql, p);
            return result > 0;
        }
    }
}
