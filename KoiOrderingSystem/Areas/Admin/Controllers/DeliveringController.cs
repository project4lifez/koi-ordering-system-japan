using KoiOrderingSystem.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    public class DeliveringController : BaseController
    {
        [Area("Admin")]

        public IActionResult Index()
        {
            return View();
        }
    }
}
