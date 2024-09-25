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
                    // Store the username and full name in session
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Lastname", user.Lastname); // Assuming FullName is a property in your Account model

                    // Set RoleId; use the null-coalescing operator to handle possible nulls
                    HttpContext.Session.SetInt32("RoleId", user.RoleId ?? 0); // Default to 0 if null

                    // Redirect based on role_id
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
            return RedirectToAction("", "Home"); // Redirect to login page after logout
        }
    }
}
