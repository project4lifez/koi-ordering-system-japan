using KoiOrderingSystem.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class PoDetailController : BaseController
    {
        public IActionResult PoDetail()
        {
            return View();
        }
    }
}
