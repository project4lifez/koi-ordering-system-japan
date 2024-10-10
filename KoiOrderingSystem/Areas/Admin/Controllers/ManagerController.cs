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

    }
}

