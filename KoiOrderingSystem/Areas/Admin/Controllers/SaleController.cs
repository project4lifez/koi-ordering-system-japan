using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("admin")]

    public class SaleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Quote()
        {
            return View();
        }
    }
}
