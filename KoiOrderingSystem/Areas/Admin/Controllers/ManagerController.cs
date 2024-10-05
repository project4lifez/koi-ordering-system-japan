using Microsoft.AspNetCore.Mvc;

namespace KoiAdmin.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ManagerController : Controller
    {
        public IActionResult Manager()
        {
         return View();
        }
        public IActionResult Feedback()
        {
            return View();
        }
    }
}
    

