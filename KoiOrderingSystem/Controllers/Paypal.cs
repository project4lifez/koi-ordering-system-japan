using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Controllers
{
    public class Paypal : Controller
    {
        public IActionResult Payment()
        {
            return View();
        }
    }
}
