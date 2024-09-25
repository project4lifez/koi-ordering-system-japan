using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Controllers
{
    public class RegisterController : Controller
    {
        private readonly Koi88Context _db;

        public RegisterController(Koi88Context db)
        {
            _db = db;
        }

        // GET: /Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Register
        [HttpPost]
        public async Task<IActionResult> Register(Account model)
        {
            if (ModelState.IsValid)
            {
                // Check if the username already exists
                var existingUser = await _db.Accounts.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View(model);
                }

                // Create new user
                model.RoleId = 1; // Set RoleId to 1 for customer
                _db.Accounts.Add(model);
                await _db.SaveChangesAsync();

                // Optionally, redirect to a login page or home page after successful registration
                return RedirectToAction("", "Login");
            }

            return View(model); // If model state is not valid, return the same view
        }
    }
}
