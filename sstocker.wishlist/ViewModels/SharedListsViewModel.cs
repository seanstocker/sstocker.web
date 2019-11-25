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
            ColumnNames = new List<string> { "Name", "Description", "Link" };
            var sharedAccountIds = WishlistRepository.GetSharedAccountIds(accountId);

            foreach (var sharedAccountId in sharedAccountIds)
                SharedWishLists.Add(new SharedWishList(sharedAccountId));

            SetBaseViewModel(accountId);
        }
    }

    public class SharedWishList
    {
        public string Name;
        public List<Wishlist> WishList;

        public SharedWishList(long sharedAccountId)
        {
            Name = AccountRepository.GetAccount(sharedAccountId).Name;
            WishList = WishlistRepository.GetWishlist(sharedAccountId);
        }

        public string GetColumn(int index, int columnLocation)
        {
            switch (columnLocation)
            {
                case 0:
                    return WishList[index].Name;
                case 1:
                    return WishList[index].Description ?? "";
                case 2:
                    return string.IsNullOrWhiteSpace(WishList[index].Link)
                        ? ""
                        : $"<a href=\"{WishList[index].Link}\">{WishList[index].Link}</a>";
                default:
                    throw new Exception("Not a valid column location.");
            }
        }
    }
}