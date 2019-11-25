using sstocker.core.Repositories;
using sstocker.core.ViewModels;
using sstocker.wishlist.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sstocker.wishlist.ViewModels
{
    public class DashboardModel : BaseViewModel
    {
        public int DaysTillNextHoliday;
        public string NextHoliday;
        public List<ListTable> FirstColumn;
        public List<ListTable> SecondColumn;

        public DashboardModel(long accountId)
        {
            SetBaseViewModel(accountId);
            FirstColumn = new List<ListTable>();
            SecondColumn = new List<ListTable>();

            FirstColumn.Add(new ListTable(accountId, true));

            var sharedAccountIds = WishlistRepository.GetSharedAccountIds(accountId);
            foreach (var sharedAccountId in sharedAccountIds)
            {
                if (FirstColumn.Sum(c => c.TableItems.Count) > SecondColumn.Sum(c => c.TableItems.Count))
                    SecondColumn.Add(new ListTable(sharedAccountId, false));
                else
                    FirstColumn.Add(new ListTable(sharedAccountId, false));
            }

            //Change this to next holiday
            //Currently set to Christmas as default
            DaysTillNextHoliday = GetDaysTill(new DateTime(DateTime.Today.Year, 12, 25));
            NextHoliday = "Christmas";
        }

        private int GetDaysTill(DateTime holiday)
        {
            var today = DateTime.Today;
            holiday = new DateTime(today.Year, holiday.Month, holiday.Day);
            var next = holiday.AddYears(today.Year - holiday.Year);

            if (next < today)
                next = next.AddYears(1);

            return (next - today).Days;
        }
    }

    public class ListTable
    {
        public string TableName;
        public string TableActionLink;
        public string TableControllerLink;
        public List<string> TableItems;

        public ListTable(long accountId, bool yourAccount)
        {
            if(yourAccount)
            {
                TableName = "Your Account";
                TableActionLink = "YourList";
                TableControllerLink = "List";
                TableItems = WishlistRepository.GetWishlist(accountId).Select(i => i.Name).ToList();
            }
            else
            {
                TableName = $"{AccountRepository.GetAccount(accountId).Name}'s List";
                TableActionLink = "SharedLists";
                TableControllerLink = "List";
                TableItems = WishlistRepository.GetWishlist(accountId).Where(i => !i.IsBought).Select(i => i.Name).ToList();
            }
        }
    }
}