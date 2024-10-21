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
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            // Check if the RoleId is null or not equal to 2
            if (adminRoleId == null || adminRoleId != 2)
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

                // Set a success message in TempData with updated status
                TempData["SuccessMessage"] = $"Status updated to '{status}' successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Booking not found.";
            }

            // Redirect back to the Manager page with updated status
            return Redirect("Manager?id=" + bookingId);
        }



        public IActionResult KoiVarietyList()
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
                return Redirect("/Admin/Manager/KoiVarietyList");
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
            return Redirect("/Admin/Manager/KoiVarietyList");
        }


        [HttpGet]
        public IActionResult UpdateVariety(int id)
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
            return Redirect($"/Admin/Manager/UpdateKoiVariety?id={id}");
        }

        public IActionResult KoiFishList()
        {
            var koiFishes = _db.KoiFishes
                             .Include(k => k.Variety) // Lấy thông tin Variety liên quan
                             .ToList();
            return View(koiFishes);
        }


        [HttpGet]
        public IActionResult CreateKoiFish()
        {
            // Fetch the list of varieties from the database
            var varieties = _db.Varieties.ToList();

            // Pass the list of varieties to the view using ViewBag
            ViewBag.Varieties = varieties;

            // Return the view for koi fish creation
            return View();
        }

        [HttpPost]
        public IActionResult AddKoiFish(KoiFish model, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                // Handle the image upload
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    // Create a unique file name to avoid overwriting files with the same name
                    var fileName = Path.GetFileNameWithoutExtension(ImageUrl.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(ImageUrl.FileName);
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/KoiFish");
                    var filePath = Path.Combine(directoryPath, fileName);

                    // Ensure the directory exists
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageUrl.CopyTo(stream);
                    }

                    // Set the image URL in the model (relative path)
                    model.ImageUrl = "/images/KoiFish/" + fileName;
                }

                // Add the new variety to the database
                _db.KoiFishes.Add(model);
                _db.SaveChanges();


                return Redirect("/Admin/Manager/KoiFishList");
            }


            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteKoiFish(int id)
        {
            var koiFishes = _db.KoiFishes.Find(id);
            if (koiFishes != null)
            {
                _db.KoiFishes.Remove(koiFishes);
                _db.SaveChanges();
            }

            // Redirect to the list of varieties
            return Redirect("/Admin/Manager/KoiFishList");
        }

        public IActionResult UpdateKoiFish(int id)
        {
            var koiFish = _db.KoiFishes.Find(id);
            if (koiFish == null)
            {
                return NotFound();
            }

            // Fetch the list of varieties to populate the dropdown
            var varieties = _db.Varieties.ToList();

            // Prepare a ViewModel (or use ViewBag) to pass data to the view
            ViewBag.Varieties = varieties;
            return View(koiFish);
        }

        [HttpPost]
        public IActionResult EditKoiFish(int id, KoiFish model, IFormFile ImageUrl)
        {

            var existingKoiFish = _db.KoiFishes.FirstOrDefault(k => k.KoiId == id);

            if (existingKoiFish == null)
            {
                return NotFound();
            }


            if (!string.IsNullOrWhiteSpace(model.KoiName))
            {
                existingKoiFish.KoiName = model.KoiName;
            }

            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                existingKoiFish.Description = model.Description;
            }

            // Cập nhật VarietyId và VarietyName
            if (model.VarietyId != 0)
            {
                existingKoiFish.VarietyId = model.VarietyId;

                // Cập nhật VarietyName từ variety model
                var variety = _db.Varieties.FirstOrDefault(v => v.VarietyId == model.VarietyId);
                if (variety != null)
                {
                    existingKoiFish.Variety.VarietyName = variety.VarietyName;
                }
            }


            if (ImageUrl != null && ImageUrl.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(ImageUrl.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(ImageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/KoiFish", fileName);

                // Lưu ảnh mới vào server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageUrl.CopyTo(stream);
                }

                // Cập nhật URL ảnh trong cơ sở dữ liệu với ảnh mới
                existingKoiFish.ImageUrl = "/images/KoiFish/" + fileName;
            }


            _db.SaveChanges();


            return Redirect($"/Admin/Manager/UpdateKoiFish?id={id}");
        }

        public IActionResult KoiFarmList()
        {
            var koiFarms = _db.KoiFarms
                     .Include(farm => farm.SpecialVarieties) // Include SpecialVarieties associated with the farm
                     .ThenInclude(sv => sv.Variety)          // Include the Variety through SpecialVariety
                     .ToList();

            return View(koiFarms);
        }

        [HttpGet]
        public IActionResult CreateFarm()
        {

            var varieties = _db.Varieties.ToList();


            ViewBag.Varieties = varieties;


            return View();
        }
        [HttpPost]
        public IActionResult AddKoiFarm(KoiFarm model, IFormFile imageUrl, List<int> selectedVarietyIds)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (imageUrl != null && imageUrl.Length > 0)
                {
                    // Create a unique file name to avoid overwriting
                    var fileName = Path.GetFileNameWithoutExtension(imageUrl.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(imageUrl.FileName);
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/KoiFarm");
                    var filePath = Path.Combine(directoryPath, fileName);

                    // Create directory if it does not exist
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Save the image file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageUrl.CopyTo(stream);
                    }

                    model.ImageUrl = Url.Content("~/images/KoiFarm/" + fileName); // Use Url.Content to handle URL properly
                }

                // Add the KoiFarm to the database
                _db.KoiFarms.Add(model);
                _db.SaveChanges(); // Save to generate FarmId

                // Create SpecialVarieties based on selected varieties
                if (selectedVarietyIds != null && selectedVarietyIds.Count > 0)
                {
                    foreach (var varietyId in selectedVarietyIds)
                    {
                        var specialVariety = new SpecialVariety
                        {
                            FarmId = model.FarmId, // Assuming FarmId is generated after SaveChanges
                            VarietyId = varietyId
                        };
                        _db.SpecialVarieties.Add(specialVariety);
                    }
                    _db.SaveChanges(); // Save the SpecialVarieties to the database
                }

                return Redirect("/Admin/Manager/KoiFarmList");
            }

            // If the model state is invalid, return to the create view with the current model
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteKoiFarm(int id)
        {
            var farmToDelete = _db.KoiFarms
                .Include(f => f.SpecialVarieties) // Include related SpecialVarieties
                .FirstOrDefault(f => f.FarmId == id);

            if (farmToDelete == null)
            {
                return NotFound();
            }

            // Remove associated SpecialVariety records first
            _db.SpecialVarieties.RemoveRange(farmToDelete.SpecialVarieties);

            // Then remove the KoiFarm
            _db.KoiFarms.Remove(farmToDelete);

            _db.SaveChanges();

            return Redirect("/Admin/Manager/KoiFarmList");
        }


        [HttpGet]
        public IActionResult UpdateFarm(int id)
        {
            // Fetch the KoiFarm with its associated SpecialVarieties
            var model = _db.KoiFarms
                .Include(farm => farm.SpecialVarieties)
                .ThenInclude(sv => sv.Variety)
                .FirstOrDefault(farm => farm.FarmId == id);

            if (model == null)
            {
                return NotFound();
            }

            // Get the list of varieties for the checkbox list
            var varieties = _db.Varieties.ToList();
            ViewBag.Varieties = varieties;

            return View(model);
        }

        [HttpPost]
        public IActionResult EditKoiFarms(int id, KoiFarm model, IFormFile ImageUrl, int[] selectedVarietyIds)
        {
            // Fetch the existing KoiFarm from the database
            var existingKoiFarm = _db.KoiFarms
                .Include(farm => farm.SpecialVarieties)
                .ThenInclude(sv => sv.Variety)
                .FirstOrDefault(farm => farm.FarmId == id);

            if (existingKoiFarm == null)
            {
                return NotFound();
            }

            // Update properties of the existing KoiFarm
            if (!string.IsNullOrWhiteSpace(model.FarmName))
            {
                existingKoiFarm.FarmName = model.FarmName;
            }

            if (!string.IsNullOrWhiteSpace(model.Location))
            {
                existingKoiFarm.Location = model.Location;
            }

            if (!string.IsNullOrWhiteSpace(model.ContactInfo))
            {
                existingKoiFarm.ContactInfo = model.ContactInfo;
            }

            // Convert selectedVarietyIds to a list for easier manipulation
            var selectedVarietiesList = selectedVarietyIds?.ToList() ?? new List<int>();

            // Update VarietyId for existing SpecialVarieties
            foreach (var specialVariety in existingKoiFarm.SpecialVarieties.ToList())
            {
                if (selectedVarietiesList.Contains((int)specialVariety.VarietyId))
                {
                    // If it is, keep it as is (no need to change)
                    continue;
                }
                else
                {
                    // If it's not in the selected variety IDs, you may want to remove it
                    _db.SpecialVarieties.Remove(specialVariety);
                }
            }

            // If you want to add new special varieties based on the selected IDs
            foreach (var selectedVarietyId in selectedVarietiesList)
            {
                // Check if the special variety already exists
                if (!existingKoiFarm.SpecialVarieties.Any(sv => sv.VarietyId == selectedVarietyId))
                {
                    // If it doesn't exist, create a new SpecialVariety
                    existingKoiFarm.SpecialVarieties.Add(new SpecialVariety
                    {
                        FarmId = existingKoiFarm.FarmId,
                        VarietyId = selectedVarietyId
                    });
                }
            }

            // Handle image upload if provided
            if (ImageUrl != null && ImageUrl.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(ImageUrl.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(ImageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/KoiFarm", fileName);

                // Save the new image to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageUrl.CopyTo(stream);
                }

                // Update the ImageUrl in the KoiFarm
                existingKoiFarm.ImageUrl = "/images/KoiFarm/" + fileName;
            }

            // Save changes to the database
            _db.SaveChanges();

            // Redirect to the appropriate page
            return Redirect($"/Admin/Manager/UpdateFarm?id={id}");
        }

        public async Task<IActionResult> Feedback(int page = 1, int pageSize = 8)
        {
            // Query the feedbacks with related customer and trip information
            var feedbacks = await _db.Feedbacks
                                     .Include(f => f.Customer)
                                     .Include(f => f.Bookings)
                                     .ThenInclude(b => b.Trip)
                                     .OrderByDescending(f => f.Feedbackdate)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

            // Get the total number of feedbacks for pagination
            var totalFeedbacks = await _db.Feedbacks.CountAsync();

            // Send feedbacks and pagination info to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalFeedbacks / (double)pageSize);

            return View(feedbacks);
        }


        public IActionResult FeedbackDetail(int feedbackId)
        {
            var feedback = _db.Feedbacks
                .Include(f => f.Bookings)
                .ThenInclude(b => b.Trip)
                .FirstOrDefault(f => f.FeedbackId == feedbackId);

            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback); // Pass the feedback object to the view
        }




    }
}







