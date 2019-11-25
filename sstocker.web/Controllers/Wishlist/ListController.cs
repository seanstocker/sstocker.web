using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sstocker.core.Helpers;
using sstocker.wishlist.Helpers;
using sstocker.wishlist.Repositories;
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

            var model = new YourListViewModel(accountId);
            return View(SettingsHelper.GetListControllerViewPath("YourList"), model);
        }

        public IActionResult SharedLists()
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.WishlistApp });

            var model = new SharedListsViewModel(accountId);
            return View(SettingsHelper.GetListControllerViewPath("SharedLists"), model);
        }

        public IActionResult AddWishlistItem()
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.WishlistApp });

            var model = new AddWishlistItemViewModel(accountId);
            return View(SettingsHelper.GetListControllerViewPath("AddWishlistItem"), model);
        }

        public ActionResult SaveWishlistItem(string name, string description, string link)
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.BudgetApp });

            if (string.IsNullOrWhiteSpace(name))
                return Json(new { status = false, message = "Name is required." });
            if (string.IsNullOrWhiteSpace(description))
                description = null;
            if (string.IsNullOrWhiteSpace(link))
                link = null;

            if (WishlistRepository.WishlistItemExists(accountId, name))
                return Json(new { status = false, message = $"Item already exists with name {name}." });

            WishlistRepository.AddWishlistItem(accountId, name, description, link);
            return Json(new { status = true, message = "Wishlist Item Added!" });
        }
    }
}