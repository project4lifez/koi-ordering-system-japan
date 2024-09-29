using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Http; // For session management
using System.Linq;
using Microsoft.EntityFrameworkCore; // For database querying
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;

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
                return RedirectToAction("", "Home");
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

                    // Get CustomerId from the Customer table based on AccountId
                    var customer = _db.Customers.FirstOrDefault(c => c.AccountId == user.AccountId);
                    if (customer == null)
                    {
                        ViewBag.Error = "No associated customer found. Please contact support.";
                        return View(model);
                    }

                    // Store the username and last name in session
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Lastname", user.Lastname);

                    // Store the Status as an integer (0 = false, 1 = true)
                    HttpContext.Session.SetInt32("Status", user.Status == true ? 1 : 0);

                    // Store the CustomerId in session
                    HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);

                    // Set RoleId; use the null-coalescing operator to handle possible nulls
                    HttpContext.Session.SetInt32("RoleId", user.RoleId ?? 0);

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

        public async Task GoogleLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleRespone")
                });
        }


        public async Task<IActionResult> GoogleRespone()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result.Principal != null)
            {
                // Extract the email, first name, and last name from the Google response
                var email = result.Principal.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
                var firstName = result.Principal.FindFirst(c => c.Type == ClaimTypes.GivenName)?.Value;
                var lastName = result.Principal.FindFirst(c => c.Type == ClaimTypes.Surname)?.Value;

                // Check if the email is null or empty before proceeding
                if (string.IsNullOrEmpty(email))
                {
                    ViewBag.Error = "Email not found in Google response.";
                    return View("Login");
                }

                // Check if the user exists in the database
                var existingUser = _db.Accounts.FirstOrDefault(u => u.Email == email);

                if (existingUser == null)
                {
                    // Create a new user if not found
                    var newUser = new Account
                    {
                        Username = email, // Use email as Username
                        Email = email,
                        Firstname = firstName ?? string.Empty, // Assign empty string if null
                        Lastname = lastName ?? string.Empty,   // Assign empty string if null
                        Status = true, // Assuming active account
                        RoleId = 1     // Assuming RoleId = 1 is for customers
                    };

                    _db.Accounts.Add(newUser);
                    _db.SaveChanges();

                    existingUser = newUser;

                    // Create a new customer entry for RoleId = 1 (Customer)
                    if (newUser.RoleId == 1)
                    {
                        var newCustomer = new Customer
                        {
                            AccountId = newUser.AccountId,
                            // Add other customer-specific information here
                        };

                        _db.Customers.Add(newCustomer);
                        _db.SaveChanges();

                        // Store CustomerId in session
                        HttpContext.Session.SetInt32("CustomerId", newCustomer.CustomerId);
                    }
                }
                else
                {
                    // If user already exists, get associated CustomerId (if RoleId == 1)
                    var customer = _db.Customers.FirstOrDefault(c => c.AccountId == existingUser.AccountId);
                    if (customer != null)
                    {
                        HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
                    }
                }

                // Save user info into session safely, with null checks
                HttpContext.Session.SetString("Username", existingUser.Username ?? string.Empty);
                HttpContext.Session.SetString("Lastname", existingUser.Lastname ?? string.Empty);
                HttpContext.Session.SetInt32("Status", existingUser.Status == true ? 1 : 0);
                HttpContext.Session.SetInt32("RoleId", existingUser.RoleId ?? 0);
            }
            else
            {
                // Handle the case where authentication fails
                ViewBag.Error = "Authentication failed.";
                return View("Login");
            }

            // Redirect to home or appropriate page
            return RedirectToAction("Login", "Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("", "Home");
        }
    }
}
