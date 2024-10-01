using Microsoft.AspNetCore.Mvc;

namespace KoiAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManagerController : Controller
    {
        public IActionResult Manager()
        {
           

            return View();
        }
    }
}
    

