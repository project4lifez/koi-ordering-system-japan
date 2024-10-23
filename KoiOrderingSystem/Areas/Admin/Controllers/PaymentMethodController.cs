using KoiOrderingSystem.Controllers.Admin;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]


    public class PaymentMethodController : BaseController
    {
        private readonly Koi88Context _db;

        public PaymentMethodController(Koi88Context db)
        {
            _db = db;
        }
        public IActionResult PaymentMethod()
        {
            var payments = _db.PaymentMethods.ToList();
            return View(payments);
        }

        [HttpGet]
        public IActionResult CreatePayment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePayment(string MethodName, string Description)
        {
            if (string.IsNullOrEmpty(MethodName))
            {
                ModelState.AddModelError("", "Payment method name is required.");
                return View("CreatePayment");
            }

            // Create a new payment method instance and save it to the database
            var newPaymentMethod = new PaymentMethod
            {
                MethodName = MethodName,
                Description = Description
            };

            _db.PaymentMethods.Add(newPaymentMethod);
            _db.SaveChanges();

            return RedirectToAction("PaymentMethod", "Admin"); 
        }

        [HttpPost]
        public IActionResult DeletePaymentMethod(int id)
        {
            // Find the role by ID
            var role = _db.PaymentMethods.Find(id);
            if (role == null)
            {
                return NotFound();
            }

            _db.PaymentMethods.Remove(role);
            _db.SaveChanges();

            return RedirectToAction("PaymentMethod", "Admin");
        }

        [HttpGet]
        public IActionResult UpdatePayment(int id)
        {
            var role = _db.PaymentMethods.Find(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost]
        public IActionResult UpdatePayment(PaymentMethod updatedPaymentMethod)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedPaymentMethod);
            }

            var existingRole = _db.PaymentMethods.Find(updatedPaymentMethod.PaymentMethodId);
            if (existingRole == null)
            {
                return NotFound();
            }

            existingRole.MethodName = updatedPaymentMethod.MethodName;
            existingRole.Description = updatedPaymentMethod.Description;
            
            _db.SaveChanges();
            return Redirect($"/Admin/PaymentMethod/UpdatePayment/{updatedPaymentMethod.PaymentMethodId}");
        }
    }
}
