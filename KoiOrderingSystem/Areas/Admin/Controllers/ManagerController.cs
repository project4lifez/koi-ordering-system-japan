using KoiOrderingSystem.Controllers.Admin;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManagerController : BaseController
    {
        private readonly Koi88Context _db;

        public ManagerController(Koi88Context db)
        {
            _db = db;
        }

        // Action to display the Manager page
        public IActionResult Manager(int id)
        {
            // Retrieve the RoleId from the session
            var roleId = HttpContext.Session.GetInt32("RoleId");

            // Check if the RoleId is null or not equal to 2
            if (roleId == null || roleId != 2)
            {
                return NotFound("You do not have permission to access this page.");
            }

            // Fetch the booking along with the related trip using Include
            var booking = _db.Bookings
                .Include(b => b.Trip)
                .ThenInclude(t => t.TripDetails)
                .Include(b => b.Po)                      // Include the related PO
                .ThenInclude(po => po.Podetails)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }

            // Pass the booking information to the view
            return View(booking);
        }


        // Action to update status
        [HttpPost]
        public IActionResult UpdateStatusManager(int bookingId, string status)
        {
            // Fetch the booking by ID
            var booking = _db.Bookings
                             .Include(b => b.BookingPayments) // Include BookingPayments to check existing payments
                             .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking != null)
            {
                // Prevent updates if the current status is "Requested"
                if (booking.Status == "Requested")
                {
                    // Set error message in TempData and redirect to Manager page
                    ModelState.AddModelError("", "Cannot update status from 'Requested'.");
                    return View("Manager", booking);
                }

                // Update the booking status
                
                    booking.Status = status;
                

                // Update QuoteSentDate
                booking.QuoteSentDate = DateOnly.FromDateTime(DateTime.Now);

                // Update QuoteApprovedDate and handle BookingPayment creation
                if (status == "Accepted")
                {
                    booking.QuoteApprovedDate = DateOnly.FromDateTime(DateTime.Now);

                    // Check if a BookingPayment already exists for this booking
                    var existingPayment = booking.BookingPayments.FirstOrDefault();
                    if (existingPayment == null)
                    {
                        // Create a new BookingPayment if it doesn't exist
                        var bookingPayment = new BookingPayment
                        {
                            Status = "Pending", // Set the initial status
                            BookingId = booking.BookingId // Assign the BookingId to the new BookingPayment
                        };

                        // Add the new BookingPayment to the context
                        _db.BookingPayments.Add(bookingPayment);
                    }
                }

                // Prevent QuoteApprovedDate from updating if status is "Rejected" or "Canceled"
                if (status == "Rejected" || status == "Canceled")
                {
                    booking.QuoteApprovedDate = null; // Optionally reset it
                }

                // Save the changes
                _db.SaveChanges();
            }

            // Redirect back to the Manager page with updated status
            return Redirect("Manager?id=" + bookingId);
        }

        public IActionResult ListOfVariety()
        {
            // Fetch specific fields but still create full Variety objects
            var varieties = _db.Varieties
                .Select(v => new Variety
                {
                    VarietyId = v.VarietyId,
                    VarietyName = v.VarietyName,
                    ImageUrl = v.ImageUrl
                })
                .ToList();

            return View(varieties);
        }


        public IActionResult CreateVariety()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddVariety(Variety model, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                // Handle the image upload
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    // Create a unique file name to avoid overwriting files with the same name
                    var fileName = Path.GetFileNameWithoutExtension(ImageUrl.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(ImageUrl.FileName);
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/KoiVarieties");
                    var filePath = Path.Combine(directoryPath, fileName);

                    // Ensure the directory exists
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Save the image file to the specified path
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageUrl.CopyTo(stream);
                    }

                    // Set the image URL in the model (relative path)
                    model.ImageUrl = "/images/KoiVarieties/" + fileName;
                }

                // Add the new variety to the database
                _db.Varieties.Add(model);
                _db.SaveChanges();

                // Redirect to the list of varieties after successful creation
                return Redirect("/Admin/Manager/ListOfVariety");
            }

            // If model state is not valid, return the view with the current model
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteVariety(int id)
        {
            var variety = _db.Varieties.Find(id);
            if (variety != null)
            {
                _db.Varieties.Remove(variety);
                _db.SaveChanges();
            }

            // Redirect to the list of varieties
            return Redirect("/Admin/Manager/ListOfVariety");
        }



        [HttpGet]
        public IActionResult Edits(int id)
        {
            var variety = _db.Varieties.Find(id); 
            if (variety == null)
            {
                return NotFound();
            }
            return View(variety); 
        }

        [HttpPost]
        public IActionResult UpdateVariety(int id, Variety model, IFormFile ImageUrl)
        {
            // Tìm variety hiện tại theo ID
            var existingVariety = _db.Varieties.FirstOrDefault(v => v.VarietyId == id);

            if (existingVariety == null)
            {
                return NotFound(); 
            }

           
            if (!string.IsNullOrWhiteSpace(model.VarietyName))
            {
                existingVariety.VarietyName = model.VarietyName;
            }

           
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                existingVariety.Description = model.Description;
            }

            // Xử lý upload ảnh nếu có ảnh mới được cung cấp
            if (ImageUrl != null && ImageUrl.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(ImageUrl.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(ImageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/KoiVarieties", fileName);

                // Lưu ảnh mới vào server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageUrl.CopyTo(stream);
                }

                // Cập nhật URL ảnh trong cơ sở dữ liệu với ảnh mới
                existingVariety.ImageUrl = "/images/KoiVarieties/" + fileName;
            }
           

            // Lưu thay đổi vào cơ sở dữ liệu
            _db.SaveChanges();

            // Chuyển hướng đến trang Edits sau khi cập nhật thành công
            return Redirect($"/Admin/Manager/Edits?id={id}");
        }

    }
}


