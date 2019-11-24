namespace sstocker.wishlist.Helpers
{
    public static class SettingsHelper
    {
        public const string CategorySettingKey = "CATEGORY";
        public const string ControllerRoute = "Wishlist/[controller]/[action]";

        public static string GetWishlistHomeControllerViewPath(string action = "Index")
        {
            return GetViewPath("WishlistHome", action);
        }

        public static string GetListControllerViewPath(string action = "Index")
        {
            return GetViewPath("List", action);
        }

        private static string GetViewPath(string controller, string action)
        {
            return $"Views/Wishlist/{controller}/{action}.cshtml";
        }
    }
}
