using KoiOrderingSystem.Controllers.Admin;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProfileController : BaseController
    {
        private readonly Koi88Context _db;

        public ProfileController(Koi88Context db)
        {
            _db = db;
        }
        public async Task<IActionResult> Profile()
        {
            // Kiểm tra session AdminSession để đảm bảo đã đăng nhập
            if (HttpContext.Session.GetString("AdminSession") == null)
            {
                return RedirectToAction("", "Login");
            }

            // Lấy AdminRoleId từ session
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId == null)
            {
                return RedirectToAction("", "Login");
            }

            // Lấy thông tin admin từ database (dựa vào AdminRoleId)
            var admin = await _db.Accounts
                                .Include(a => a.Role) // Nếu có liên kết với Account hoặc thông tin khác
                                .FirstOrDefaultAsync(a => a.RoleId == adminRoleId); // Use RoleId to find the admin

            if (admin == null)
            {
                return NotFound();
            }

            ViewBag.Email = admin.Email;
            ViewBag.FirstName = admin.Firstname;
            ViewBag.LastName = admin.Lastname;
            ViewBag.Gender = admin.Gender; // Assuming Gender is stored as a string or nullable int in the database
            ViewBag.PhoneNumber = admin.Phone;
            ViewBag.Role = admin.Role.Name;
            ViewBag.ImageUrl = admin.ImageUrl;

            // Truyền dữ liệu admin vào view để hiển thị thông tin
            return View(admin);
        }

        [HttpPost]
        public IActionResult UpdateProfile(string firstname, string lastname, string gender, string phone, string email)
        {
            // Retrieve AdminRoleId from the session (instead of from the form)
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch the existing admin data based on the RoleId
            var admin = _db.Accounts.Include(a => a.Role)
                                     .FirstOrDefault(a => a.RoleId == adminRoleId);

            if (admin != null)
            {
                // Update fields only if they are provided
                if (!string.IsNullOrWhiteSpace(firstname))
                {
                    admin.Firstname = firstname;
                }

                if (!string.IsNullOrWhiteSpace(lastname))
                {
                    admin.Lastname = lastname;
                    HttpContext.Session.SetString("AdminLastname", lastname); // Update session with new lastname
                }

                if (!string.IsNullOrWhiteSpace(gender))
                {
                    admin.Gender = gender;
                }

                if (!string.IsNullOrWhiteSpace(phone))
                {
                    admin.Phone = phone;
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    admin.Email = email; // Update the email
                }

                // Save the changes to the database
                _db.SaveChanges();
            }

            // Redirect back to the profile page
            return RedirectToAction("Profile", "Admin", new { area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> SaveAvatar(IFormFile avatar)
        {
            // Retrieve AdminRoleId from the session
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch the existing admin based on AdminRoleId
            var admin = await _db.Accounts
                                 .Include(a => a.Role) // Include role if necessary
                                 .FirstOrDefaultAsync(a => a.RoleId == adminRoleId);

            if (admin == null)
            {
                return NotFound(); // Return a 404 if the admin is not found
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
                admin.ImageUrl = "/images/avatars/" + uniqueFileName;
                await _db.SaveChangesAsync();
            }

            // Redirect to the profile page with the updated avatar
            return RedirectToAction("Profile", "Admin", new { area = "" });
        }

    }
}
