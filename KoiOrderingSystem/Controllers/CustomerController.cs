using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult YourOrder()
        {
            return View();
        }
        public IActionResult TripDetails()
        {
            return View();
        }
    }
}
