using Microsoft.AspNetCore.Mvc;

namespace sstocker.web.Controllers.Base
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}