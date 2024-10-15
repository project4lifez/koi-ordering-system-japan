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
                // Check user in the database
                var user = _db.Accounts
                              .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Check if the account is disabled
                    if (user.Status == false)
                    {
                        ViewBag.Error = "Your account has been disabled. Please contact support.";
                        return View(model);
                    }

                    // Store information in session
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Lastname", user.Lastname);
                    HttpContext.Session.SetInt32("Status", user.Status == true ? 1 : 0);
                    HttpContext.Session.SetInt32("RoleId", user.RoleId ?? 0);

                    // Check for specific roles and store admin session if applicable
                    if (user.RoleId >= 2 && user.RoleId <= 5) // Admin/Staff
                    {
                        HttpContext.Session.SetString("AdminSession", user.Username);
                        
                        HttpContext.Session.SetInt32("AdminRoleId", user.RoleId ?? 0);
                        
                        HttpContext.Session.SetString("AdminLastname", user.Lastname);



                        // Store AdminRoleId
                    }
                    else if (user.RoleId == 1) // Customer
                    {
                        // Retrieve CustomerId from the Customers table
                        var customer = _db.Customers.FirstOrDefault(c => c.AccountId == user.AccountId);
                        if (customer == null)
                        {
                            ViewBag.Error = "No associated customer found. Please contact support.";
                            return View(model);
                        }

                        // Store CustomerId and Username in session for customers
                        HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
                        HttpContext.Session.SetString("CustomerLastName", user.Lastname);
                        HttpContext.Session.SetString("CustomerSession", user.Username);
                        HttpContext.Session.SetInt32("CustomerRoleId",user.RoleId ?? 0);
                    }

                    // Redirect based on RoleId
                    if (user.RoleId == 1) // Customer
                    {
                        return RedirectToAction("", "Home"); // Customer Home page
                    }
                    else if (user.RoleId >= 2 && user.RoleId <= 5) // Admin/Staff
                    {
                        return RedirectToAction("Home", "Admin"); // Admin Dashboard page
                    }
                }
                else
                {
                    ViewBag.Error = "Invalid username or password.";
                    return View(model);
                }
            }

            return View(model);
        }

        // Initiate Google Login
        public async Task GoogleLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        // Handle Google response
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result.Principal != null)
            {
                var email = result.Principal.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
                var firstName = result.Principal.FindFirst(c => c.Type == ClaimTypes.GivenName)?.Value;
                var lastName = result.Principal.FindFirst(c => c.Type == ClaimTypes.Surname)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    ViewBag.Error = "Email not found in Google response.";
                    return View("Login");
                }

                // Tìm tài khoản dựa trên email
                var existingUser = _db.Accounts.FirstOrDefault(u => u.Email == email);

                if (existingUser == null)
                {
                    // Tạo tài khoản mới nếu không tìm thấy
                    var newUser = new Account
                    {
                        Username = email,
                        Email = email,
                        Firstname = firstName ?? string.Empty,
                        Lastname = lastName ?? string.Empty,
                        Status = true, // Mặc định tài khoản mới được kích hoạt
                        RoleId = 1 // Chỉ cho phép RoleId là 1 (Customer) đăng nhập qua Google
                    };

                    _db.Accounts.Add(newUser);
                    _db.SaveChanges();
                    existingUser = newUser;

                    // Tạo đối tượng Customer mới nếu chưa có
                    var newCustomer = new Customer
                    {
                        AccountId = newUser.AccountId
                    };

                    _db.Customers.Add(newCustomer);
                    _db.SaveChanges();

                    // Lưu CustomerId vào session
                    HttpContext.Session.SetInt32("CustomerId", newCustomer.CustomerId);
                    HttpContext.Session.SetString("CustomerSession", newUser.Username);
                    HttpContext.Session.SetString("CustomerLastName", newUser.Lastname);
                    HttpContext.Session.SetInt32("CustomerRoleId", newUser.RoleId ?? 0); // Lưu RoleId cho khách hàng
                }
                else
                {
                    // Đảm bảo chỉ khách hàng (RoleId = 1) được phép đăng nhập qua Google
                    if (existingUser.RoleId != 1)
                    {
                        ViewBag.Error = "Only customers can log in with Google.";
                        return View("Login");
                    }

                    // Lấy CustomerId từ bảng Customers nếu người dùng là Customer
                    var customer = _db.Customers.FirstOrDefault(c => c.AccountId == existingUser.AccountId);
                    if (customer != null)
                    {
                        HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
                        HttpContext.Session.SetString("CustomerSession", existingUser.Username);
                        HttpContext.Session.SetString("CustomerLastName", existingUser.Lastname); // Lưu LastName vào session
                        HttpContext.Session.SetInt32("CustomerRoleId", existingUser.RoleId ?? 0); // Lưu RoleId vào session
                    }
                }

                // Lưu thông tin người dùng vào session
                HttpContext.Session.SetString("Username", existingUser.Username ?? string.Empty);
                HttpContext.Session.SetString("Lastname", existingUser.Lastname ?? string.Empty);
                HttpContext.Session.SetInt32("Status", existingUser.Status == true ? 1 : 0);
                HttpContext.Session.SetInt32("RoleId", existingUser.RoleId ?? 0);

                // Chỉ khách hàng (RoleId = 1) mới được phép login qua Google
                if (existingUser.RoleId == 1)
                {
                    return RedirectToAction("", "Home"); // Chuyển đến trang chủ cho khách hàng
                }
            }
            else
            {
                ViewBag.Error = "Authentication failed.";
                return View("Login");
            }

            return RedirectToAction("", "Login");
        }


        public IActionResult Logout()
        {
            // Kiểm tra RoleId trong session để xác định loại người dùng
            var roleId = HttpContext.Session.GetInt32("RoleId");

            if (roleId == 1) // Customer
            {
                // Xóa session cho Customer
                HttpContext.Session.Remove("CustomerSession");
                HttpContext.Session.Remove("CustomerId");
                HttpContext.Session.Remove("CustomerRoleId");
                HttpContext.Session.Remove("Status");
                HttpContext.Session.Remove("Username");
                HttpContext.Session.Remove("Lastname");



            }
            else if (roleId >= 2 && roleId <= 5) // Admin hoặc Staff
            {
                // Xóa session cho Admin
                HttpContext.Session.Remove("AdminSession");
                HttpContext.Session.Remove("AdminRoleId");
                HttpContext.Session.Remove("Status");
                HttpContext.Session.Remove("Username");
                HttpContext.Session.Remove("Lastname");
                HttpContext.Session.Remove("AdminLastname");


            }


            // Redirect đến trang đăng nhập
            return RedirectToAction("", "Login");
        }
    }
}
