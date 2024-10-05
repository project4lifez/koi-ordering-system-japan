using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult YourOrder()
        {
            return View();
        }
    }
}
