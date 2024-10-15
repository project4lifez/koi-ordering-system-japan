using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoiOrderingSystem.Controllers.Admin;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DeliveringController : BaseController
    {
        private readonly Koi88Context _context;

        public DeliveringController(Koi88Context context)
        {
            _context = context;
        }

        // Action to display the Delivering page
        public IActionResult Delivering(int id)
        {
            // Retrieve the RoleId from the session
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            // Check if the RoleId is null or not equal to 5
            if (adminRoleId == null || adminRoleId != 5)
            {
                return NotFound("You do not have permission to access this page.");
            }

            // Load Booking with related PO and PODetails
            var booking = _context.Bookings
                .Include(b => b.Po)                  // Include the related PO
                .ThenInclude(po => po.Podetails)     // Include the related PODetails
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }

            // Pass the booking data to the view
            return View(booking);
        }


        // Action to update status
        [HttpPost]
        public IActionResult UpdateStatusDelivering(int id, string status)
        {
            // Find the booking by ID
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == id);

            if (booking != null)
            {
                // Prevent changing status if it is not 'Delivering' when trying to set it to 'Delivered'
                if (status.Equals("Delivered", StringComparison.OrdinalIgnoreCase) &&
                    !booking.Status.Equals("Delivering", StringComparison.OrdinalIgnoreCase))
                {
                    // Add an error message to ModelState if trying to set 'Delivered' without 'Delivering'
                    ModelState.AddModelError("StatusError", "The booking must be in 'Delivering' status before updating to 'Delivered'.");

                    // Reload the view with the error message
                    return View("Delivering", booking);
                }

                // Prevent changing status if it is already 'Delivered'
                if (!booking.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                {
                    // Update the new status
                    booking.Status = status;

                    // Save changes to the database
                    _context.SaveChanges();

                    // Set success message
                    TempData["SuccessMessage"] = $"Booking status updated to '{status}' successfully.";
                }
            }

            // Redirect back to the 'Delivering' page after successful update
            return Redirect("Delivering?id=" + id);
        }
    }

}
