using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sstocker.core.Helpers;
using sstocker.wishlist.Helpers;
using sstocker.wishlist.ViewModels;

namespace sstocker.web.Controllers.Wishlist
{
    [Route(SettingsHelper.ControllerRoute)]
    public class ListController : Controller
    {
        public IActionResult YourList()
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.WishlistApp });

            var model = new YourListViewModel();
            model.SetBaseViewModel(accountId);
            return View(SettingsHelper.GetListControllerViewPath("YourList"), model);
        }

        public IActionResult SharedLists()
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.WishlistApp });

            var model = new SharedListsViewModel();
            model.SetBaseViewModel(accountId);
            return View(SettingsHelper.GetListControllerViewPath("SharedLists"), model);
        }
    }
}