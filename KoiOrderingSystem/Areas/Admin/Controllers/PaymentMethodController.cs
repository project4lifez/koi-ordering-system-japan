using KoiOrderingSystem.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]


    public class PaymentMethodController : BaseController
    {

        public IActionResult PaymentMethod()
        {
            return View();
        }
        public IActionResult CreatePayment()
        {
            return View();
        }

        public IActionResult UpdatePayment()
        {
            return View();
        }
    }
}
