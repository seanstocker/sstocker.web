using sstocker.core.ViewModels;
using sstocker.wishlist.Models;
using sstocker.wishlist.Repositories;
using System;
using System.Collections.Generic;

namespace sstocker.wishlist.ViewModels
{
    public class YourListViewModel : BaseViewModel
    {
        public List<string> ColumnNames;
        public List<Wishlist> WishList;

        public YourListViewModel(long accountId)
        {
            WishList = WishlistRepository.GetWishlist(accountId);
            ColumnNames = new List<string> { "Name", "Description", "Link" };
            SetBaseViewModel(accountId);
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