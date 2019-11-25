using sstocker.core.Repositories;
using sstocker.core.ViewModels;
using sstocker.wishlist.Models;
using sstocker.wishlist.Repositories;
using System;
using System.Collections.Generic;

namespace sstocker.wishlist.ViewModels
{
    public class SharedListsViewModel : BaseViewModel
    {
        public List<string> ColumnNames;
        public List<SharedWishList> SharedWishLists;

        public SharedListsViewModel(long accountId)
        {
            SharedWishLists = new List<SharedWishList>();
            ColumnNames = new List<string> { "Name", "Description" };
            var sharedAccountIds = WishlistRepository.GetSharedAccountIds(accountId);

            foreach (var sharedAccountId in sharedAccountIds)
                SharedWishLists.Add(new SharedWishList(sharedAccountId));

            SetBaseViewModel(accountId);
        }
    }

    public class SharedWishList
    {
        public string Name;
        public List<SpentWishlist> WishList;

        public SharedWishList(long sharedAccountId)
        {
            Name = AccountRepository.GetAccount(sharedAccountId).Name;
            WishList = WishlistRepository.GetSpentWishlist(sharedAccountId);
        }

        public string GetColumn(int index, int columnLocation)
        {
            switch (columnLocation)
            {
                case 0:
                    return string.IsNullOrWhiteSpace(WishList[index].Link)
                        ? WishList[index].Name
                        : $"<a href=\"{WishList[index].Link}\">{WishList[index].Name}</a>";
                case 1:
                    return WishList[index].Description ?? "";
                default:
                    throw new Exception("Not a valid column location.");
            }
        }
    }
}