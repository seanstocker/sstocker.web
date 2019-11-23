using Microsoft.AspNetCore.Mvc;
using sstocker.core.Helpers;
using sstocker.wishlist.Helpers;
using sstocker.wishlist.ViewModels;

namespace sstocker.web.Controllers.Wishlist
{
    [Route(SettingsHelper.ControllerRoute)]
    public class WishlistHomeController : Controller
    {
        public IActionResult Dashboard()
        {
            var accountId = HttpContext.Session.Get<long>(SessionHelper.SessionKeyAccountId);
            if (accountId == default)
                return RedirectToAction("Login", "Account", new { id = LoginHelper.WishlistApp });

            var model = new DashboardModel();
            model.SetBaseViewModel(accountId);
            return View(SettingsHelper.GetWishlistHomeControllerViewPath("Dashboard"), model);
        }
    }
}