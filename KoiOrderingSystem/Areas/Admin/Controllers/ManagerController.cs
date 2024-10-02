using KoiOrderingSystem.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;

namespace KoiAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManagerController : BaseController
    {
        public IActionResult Manager()
        {
           

            return View();
        }
    }
}
    

