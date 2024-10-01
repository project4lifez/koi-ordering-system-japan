using Microsoft.AspNetCore.Mvc;

namespace KoiAdmin.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ConsultingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
