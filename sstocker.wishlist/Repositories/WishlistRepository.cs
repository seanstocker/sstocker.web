using sstocker.core.Helpers;
using sstocker.wishlist.Models;
using System;
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

        public static List<SpentWishlist> GetSpentWishlist(long accountId)
        {
            var sql = @"
SELECT wl.*, si.GifterAccountId, a.Name [GifterAccountName]
FROM Wishlist.dbo.Wishlist wl
	LEFT JOIN Wishlist.dbo.SpentItem si
		ON wl.WishlistId = si.WishlistId
	LEFT JOIN Account.dbo.Account a
		ON si.GifterAccountId = a.AccountId
WHERE wl.AccountId = @AccountId
    AND wl.IsActive = 1";

            var p = new
            {
                AccountId = accountId
            };

            var result = DatabaseHelper.Query<SpentWishlist>(sql, p);
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

        public static void BuyWishlistItem(long wishlistId, long gifterAccountId, DateTime giftDate, DateTime spentDate)
        {
            var sql = @"
INSERT INTO Wishlist.dbo.SpentItem
VALUES (@WishlistId, @GifterAccountId, @GiftDate, @SpentDate)

UPDATE Wishlist.dbo.Wishlist
SET IsBought = 1
WHERE WishlistId = @WishlistId";

            var p = new
            {
                WishlistId = wishlistId,
                GifterAccountId = gifterAccountId,
                GiftDate = giftDate,
                SpentDate = spentDate
            };

            DatabaseHelper.Execute(sql, p);
        }
    }
}
