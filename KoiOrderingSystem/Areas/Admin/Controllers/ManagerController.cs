using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManagerController : Controller
    {
        private readonly Koi88Context _db;

        public ManagerController(Koi88Context db)
        {
            _db = db;
        }

        // Action to display the Manager page
        public IActionResult Manager(int id)
        {
            // Fetch the booking along with the related trip using Include
            var booking = _db.Bookings
                .Include(b => b.Trip) // Assuming Trip is a navigation property in Booking
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }


        // Action to update status
        [HttpPost]
        public IActionResult UpdateStatusManager(int bookingId, string status)
        {
            // Fetch the booking by ID
            var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

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

                // Update QuoteApprovedDate only if status is "Accepted"
                if (status == "Accepted")
                {
                    booking.QuoteApprovedDate = DateOnly.FromDateTime(DateTime.Now);
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
    }
}
