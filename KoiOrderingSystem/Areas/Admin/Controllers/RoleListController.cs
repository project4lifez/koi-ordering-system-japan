using KoiOrderingSystem.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class RoleListController : BaseController
    {

        public IActionResult RoleList()
        {
            return View();
        }

        public IActionResult CreateRole()
        {
            return View();
        }
        public IActionResult UpdateRole()
        {
            return View();
        }
    }
}
