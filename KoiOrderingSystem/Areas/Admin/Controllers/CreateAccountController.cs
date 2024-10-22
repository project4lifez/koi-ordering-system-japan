using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    public class CreateAccountController : Controller
    {
        public IActionResult CreateAccount()
        {
            return View();
        }
    }
}
