using sstocker.core.Helpers;
using System;

namespace sstocker.core.ViewModels
{
    public class LoginViewModel
    {
        public string ActionName;
        public string ControllerName;
        public string Site;

        public LoginViewModel(string site)
        {
            Site = site;

            switch (site)
            {
                case LoginHelper.BudgetApp:
                    ActionName = "Dashboard";
                    ControllerName = "BudgetHome";
                    break;
                case LoginHelper.WishlistApp:
                    ActionName = "Dashboard";
                    ControllerName = "WishlistHome";
                    break;
                default:
                    throw new Exception($"{site} is not a valid site.");
            }
        }
    }
}
