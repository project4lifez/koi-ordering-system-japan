using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Http; // For session management
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;

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
        [ValidateAntiForgeryToken]
        public IActionResult Login(Account model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra người dùng trong cơ sở dữ liệu
                var user = _db.Accounts
                              .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Kiểm tra nếu tài khoản bị vô hiệu hóa
                    if (user.Status == false)
                    {
                        ViewBag.Error = "Your account has been disabled. Please contact support.";
                        return View(model);
                    }

                    // Phân biệt khách hàng và các role khác (Admin, Staff)
                    if (user.RoleId == 1) // Khách hàng
                    {
                        // Lấy CustomerId từ bảng Customers dựa trên AccountId
                        var customer = _db.Customers.FirstOrDefault(c => c.AccountId == user.AccountId);
                        if (customer == null)
                        {
                            ViewBag.Error = "No associated customer found. Please contact support.";
                            return View(model);
                        }

                        // Lưu CustomerId vào session
                        HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
                    }
                    else if (user.RoleId >= 2 && user.RoleId <= 5) // Các role Admin hoặc Staff
                    {
                        // Không cần kiểm tra CustomerId cho các role Admin và Staff
                        HttpContext.Session.SetString("StaffRole", "true");  // Lưu vai trò nhân viên
                    }

                    // Lưu thông tin vào Session
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Lastname", user.Lastname);
                    HttpContext.Session.SetInt32("Status", user.Status == true ? 1 : 0);
                    HttpContext.Session.SetInt32("RoleId", user.RoleId ?? 0);

                    // Chuyển hướng dựa trên RoleId
                    if (user.RoleId == 1) // Khách hàng
                    {
                        return RedirectToAction("", "Home"); // Chuyển hướng đến trang HomePage của khách hàng
                    }
                    else if (user.RoleId >= 2 && user.RoleId <= 5) // Admin roles hoặc Staff
                    {
                        return RedirectToAction("Home", "Admin"); // Chuyển hướng đến Admin Dashboard
                    }
                }
                else
                {
                    // Thông báo lỗi nếu đăng nhập không thành công
                    ViewBag.Error = "Invalid username or password.";
                    return View(model);
                }
            }

            // Nếu ModelState không hợp lệ
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
