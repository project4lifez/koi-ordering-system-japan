using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Controllers
{
    public class ProfileController : Controller
    {
        private readonly Koi88Context _db;

        public ProfileController(Koi88Context db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CustomerProfile()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
            }

            // Get CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
            }

            var customer = _db.Customers
                              .Include(c => c.Account)
                              .FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }


        [HttpPost]
        public IActionResult UpdateProfile(string firstname, string lastname, string gender, string phone)
        {
            // Retrieve CustomerId from the session (instead of from the form)
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch the existing customer data based on the customerId
            var customer = _db.Customers.Include(c => c.Account)
                                        .FirstOrDefault(c => c.CustomerId == customerId);

            if (customer != null)
            {
                // Update fields only if they are provided
                if (!string.IsNullOrWhiteSpace(firstname))
                {
                    customer.Account.Firstname = firstname;
                }

                if (!string.IsNullOrWhiteSpace(lastname))
                {
                    customer.Account.Lastname = lastname;
                    HttpContext.Session.SetString("CustomerLastName", lastname); // Update session with new lastname

                }

                if (!string.IsNullOrWhiteSpace(gender))
                {
                    customer.Account.Gender = gender;
                }

                if (!string.IsNullOrWhiteSpace(phone))
                {
                    customer.Account.Phone = phone;
                }

                // Save the changes to the database
                _db.SaveChanges();
            }

            // Redirect back to the profile page

            return RedirectToAction("CustomerProfile", "Profile");
        }

        [HttpPost]
        public async Task<IActionResult> SaveAvatar(IFormFile avatar)
        {
            // Retrieve CustomerId from the session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
            }

            // Fetch the existing customer based on customerId
            var customer = await _db.Customers.Include(c => c.Account)
                                              .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return NotFound(); // Return a 404 if the customer is not found
            }

            if (avatar != null && avatar.Length > 0)
            {
                // Define the path to save the image
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists
                }

                // Generate a unique filename for the uploaded image
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(avatar.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the uploaded file to the server asynchronously
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(fileStream);
                }

                // Update the Account's ImageUrl property with the relative path
                customer.Account.ImageUrl = "/images/avatars/" + uniqueFileName;
                await _db.SaveChangesAsync();
            }

            // Redirect to the profile page with the updated avatar
            return RedirectToAction("CustomerProfile");

        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            // Retrieve CustomerId from the session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return Json(new { errors = new List<string> { "User not logged in." } });
            }

            // Fetch the existing customer based on customerId
            var customer = await _db.Customers.Include(c => c.Account)
                                               .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return Json(new { errors = new List<string> { "Customer not found." } }); // Return a 404 if the customer is not found
            }

            var errors = new List<string>();

            // Validate the current password
            if (currentPassword != customer.Account.Password) // Assuming Password is stored in plaintext
            {
                errors.Add("Current password is incorrect.");
            }

            // Check if new password and confirm password match
            if (newPassword != confirmPassword)
            {
                errors.Add("New password and confirm password do not match.");
            }

            // Check the length of the new password
            if (newPassword.Length < 8)
            {
                errors.Add("New password must be at least 8 characters long.");
            }

            if (errors.Any())
            {
                return Json(new { errors }); // Return errors as JSON
            }

            // Update the password
            customer.Account.Password = newPassword; // Update to the new password
            await _db.SaveChangesAsync();

            return Json(new { success = true }); // Return success response
        }






    }
}
