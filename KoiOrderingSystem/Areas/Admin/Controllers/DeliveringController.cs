using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    public class DeliveringController : Controller
    {
        [Area("admin")]

        public IActionResult Index()
        {
            return View();
        }
    }
}
