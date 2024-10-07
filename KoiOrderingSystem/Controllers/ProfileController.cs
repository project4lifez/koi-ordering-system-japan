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

        public IActionResult CustomerProfile(int customerId)
        {
            
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
        public IActionResult UpdateProfile(int customerId, string firstname, string lastname, string gender, string phone)
        {
            // Fetch the existing customer data based on the customerId
            var customer = _db.Customers.Include(c => c.Account)
                                         .FirstOrDefault(c => c.CustomerId == customerId);

            if (customer != null)
            {
                // Check if the provided fields are not empty or null before updating
                if (!string.IsNullOrWhiteSpace(firstname))
                {
                    customer.Account.Firstname = firstname;
                }

                if (!string.IsNullOrWhiteSpace(lastname))
                {
                    customer.Account.Lastname = lastname;
                }

                if (!string.IsNullOrWhiteSpace(gender))
                {
                    customer.Account.Gender = gender;
                }

                if (!string.IsNullOrWhiteSpace(phone))
                {
                    customer.Account.Phone = phone;
                }

                // Save changes to the database
                _db.SaveChanges();
            }

            // Redirect back to the profile page using customerId
            return RedirectToAction("CustomerProfile", "Profile", new { customerId = customerId });
        }

        [HttpPost]
        public async Task<IActionResult> SaveAvatar(int customerId, IFormFile avatar)
        {
            
            var customer = await _db.Customers.Include(c => c.Account)
                                               .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return NotFound();
            }

            if (avatar != null && avatar.Length > 0)
            {
                // Define the path to save the image
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars"); 
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(avatar.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the uploaded file to the server asynchronously
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(fileStream);
                }

                // Update the Account's AvatarPath property
                customer.Account.ImageUrl = "/images/avatars/" + uniqueFileName; 
                await _db.SaveChangesAsync();
            }

           
            return RedirectToAction("CustomerProfile", new { customerId });
        }
    }
}
