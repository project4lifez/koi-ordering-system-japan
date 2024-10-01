using KoiOrderingSystem.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;

namespace KoiAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ConsultingController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
