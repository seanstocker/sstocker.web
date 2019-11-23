using sstocker.core.Helpers;
using System;

namespace sstocker.core.ViewModels
{
    public class LoginViewModel
    {
        public string ActionName;
        public string ControllerName;

        public LoginViewModel(string site)
        {
            switch (site)
            {
                case LoginHelper.BudgetApp:
                    ActionName = "Dashboard";
                    ControllerName = "Home";
                    break;
                default:
                    throw new Exception($"{site} is not a valid site.");
            }
        }
    }
}
