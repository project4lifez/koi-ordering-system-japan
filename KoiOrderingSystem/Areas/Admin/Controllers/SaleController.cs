using KoiOrderingSystem.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class SaleController : BaseController
    {
        public IActionResult Sale()
        {
            return View();
        }
        public IActionResult Quote()
        {
            return View();
        }
    }
}
