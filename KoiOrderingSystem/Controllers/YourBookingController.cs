using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Http; // For session management
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Controllers
{
    public class YourBookingController : Controller
    {
        private readonly Koi88Context _db;

        public YourBookingController(Koi88Context db)
        {
            _db = db;
        }

        // Action to display the list of bookings
        public async Task<IActionResult> YourBooking()
        {
            // Check if the user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get the list of bookings for the current user
            var bookings = await _db.Bookings
                                    .Where(b => b.CustomerId == customerId.Value)
                                    .ToListAsync();

            return View(bookings); // Pass the booking list to the view
        }

        // Payment Action to display the payment form for a specific booking
        public async Task<IActionResult> Payment(int bookingId)
        {
            // Check if the user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch the specific booking for this customer
            var booking = await _db.Bookings
                                   .Include(b => b.Trip) // Include the related Trip entity so you can access TripName
                                   .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.CustomerId == customerId.Value);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Pass the booking to the Payment view
            return View(booking);
        }

        // Action to handle form postback from the payment form
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(int bookingId)
        {
            // Check if the user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Find the specific booking
            var booking = await _db.Bookings
                                  .Include(b => b.Trip) // Include the related Trip entity so you can access TripName
                                  .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.CustomerId == customerId.Value);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Simulate payment confirmation (you can add actual payment logic here)
            booking.Status = "Payment Completed";
            await _db.SaveChangesAsync();

            // Redirect to a success page or the booking details page
            return RedirectToAction("YourBooking");
        }

        // Action to resend quote request
        [HttpPost]
        public async Task<IActionResult> ResendQuote(int bookingId)
        {
            // Check if the user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Find the specific booking
            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId && b.CustomerId == customerId.Value);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Update status to "Requested"
            booking.Status = "Requested";
            await _db.SaveChangesAsync();

            return RedirectToAction("YourBooking");
        }
    }
}
