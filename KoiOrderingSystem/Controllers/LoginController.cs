using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Http; // For session management
using System.Linq;

namespace KoiOrderingSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly Koi88Context _db;

        public LoginController(Koi88Context db)
        {
            _db = db;
        }

        // GET: /Login
        [HttpGet]
        public IActionResult Login()
        {
            // Check if the user is already logged in
            if (HttpContext.Session.GetString("Username") != null)
            {
                // User is already logged in, redirect to home page
                return RedirectToAction("Index", "Home");
            }
            return View(); // Show login page if user is not logged in
        }

        // POST: /Login
        [HttpPost]
        public IActionResult Login(Account model)
        {
            if (ModelState.IsValid)
            {
                // Check if the user exists in the database
                var user = _db.Accounts
                              .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Check if the account is disabled
                    if (user.Status == false) // If Status is false
                    {
                        ViewBag.Error = "Your account has been disabled. Please contact support.";
                        return View(model);
                    }

                    // Store the username and last name in session
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Lastname", user.Lastname);

                    // Store the Status as an integer (0 = false, 1 = true)
                    HttpContext.Session.SetInt32("Status", user.Status == true ? 1 : 0);

                    // Set RoleId; use the null-coalescing operator to handle possible nulls
                    HttpContext.Session.SetInt32("RoleId", user.RoleId ?? 0); // Default to 0 if null

                    // Redirect based on RoleId
                    if (user.RoleId == 1) // Customer
                    {
                        return RedirectToAction("", "Home"); // Redirect to HomePage
                    }
                    else if (user.RoleId == 2) // Manager
                    {
                        return RedirectToAction("", "Manager");
                    }
                }
                else
                {
                    // Add error message to ViewBag if login fails
                    ViewBag.Error = "Invalid username or password.";
                    return View(model);
                }
            }

            // If the model state is not valid, return the same view
            return View(model);
        }

        public IActionResult Logout()
        {
            // Clear the session to log the user out
            HttpContext.Session.Clear();
            return RedirectToAction("", "Home"); // Redirect to home page after logout
        }
    }
}
