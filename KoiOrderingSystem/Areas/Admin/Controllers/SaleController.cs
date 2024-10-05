using KoiOrderingSystem.Controllers.Admin;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SaleController : BaseController
    {
        private readonly Koi88Context _db;

        public SaleController(Koi88Context db)
        {
            _db = db;
        }

        // Display Quote information for a given booking
        public IActionResult Quote(int bookingId)
        {
            // Retrieve booking information from the database, including related Trip and TripDetails
            var booking = _db.Bookings
                .Include(b => b.Trip)
                .ThenInclude(t => t.TripDetails)
                .FirstOrDefault(b => b.BookingId == bookingId);

            // Check if booking exists
            if (booking == null)
            {
                return NotFound($"Booking with ID {bookingId} not found.");
            }

            // Pass booking information to the view for display and editing
            return View(booking);
        }

        // Handle the POST request to create quote information with multiple trip details
        [HttpPost]
        public IActionResult CreateQuote(int bookingId, string tripName, decimal? quotedAmount, DateTime? startDate, DateTime? endDate, List<DateTime> days, List<string> mainTopics, List<string> subTopics, List<string> notePrices) // notePrices is now List<string>
        {
            // Step 1: Retrieve booking information from the database
            var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

            // Step 2: Check if booking exists
            if (booking == null)
            {
                return NotFound($"Booking with ID {bookingId} not found.");
            }

            // Step 3: Create a new Trip
            var trip = new Trip
            {
                TripName = tripName,
                TripDetails = new List<TripDetail>()
            };

            // Step 4: Create multiple TripDetails based on the user inputs
            for (int i = 0; i < days.Count; i++)
            {
                var tripDetail = new TripDetail
                {
                    Day = DateOnly.FromDateTime(days[i]),  // Single day for each TripDetail
                    MainTopic = mainTopics[i],
                    SubTopic = subTopics[i],
                    NotePrice = notePrices[i], // NotePrice is now a string
                    Trip = trip  // Associate TripDetail with the Trip
                };
                trip.TripDetails.Add(tripDetail);
            }

            // Step 5: Assign the new trip to the booking
            booking.Trip = trip;

            // Step 6: Set other booking properties
            if (startDate.HasValue)
            {
                booking.StartDate = DateOnly.FromDateTime(startDate.Value);
            }

            if (endDate.HasValue)
            {
                booking.EndDate = DateOnly.FromDateTime(endDate.Value);
            }

            if (quotedAmount.HasValue)
            {
                booking.QuotedAmount = quotedAmount.Value;
            }

            // Step 7: Save changes to the database
            _db.SaveChanges();

            // Step 8: Redirect to the quote view after creation
            return Redirect("Quote?Bookingid=" + bookingId);
        }

        // Handle the POST request to update the quote information with multiple trip details
        [HttpPost]
        public IActionResult UpdateQuote(int bookingId, string tripName, decimal? quotedAmount, DateTime? startDate, DateTime? endDate, List<DateTime> days, List<string> mainTopics, List<string> subTopics, List<string> notePrices) // notePrices is now List<string>
        {
            // Step 1: Retrieve booking information, including related Trip and TripDetails
            var booking = _db.Bookings
                             .Include(b => b.Trip)
                             .ThenInclude(t => t.TripDetails)
                             .FirstOrDefault(b => b.BookingId == bookingId);

            // Step 2: Check if booking and related data exist
            if (booking == null || booking.Trip == null)
            {
                return NotFound($"Booking or related Trip with ID {bookingId} not found.");
            }

            // Step 3: Update booking details
            if (startDate.HasValue)
            {
                booking.StartDate = DateOnly.FromDateTime(startDate.Value);
            }

            if (endDate.HasValue)
            {
                booking.EndDate = DateOnly.FromDateTime(endDate.Value);
            }

            if (quotedAmount.HasValue)
            {
                booking.QuotedAmount = quotedAmount.Value;
            }

            // Step 4: Update Trip details
            booking.Trip.TripName = tripName;

            // Clear old trip details and add new ones
            booking.Trip.TripDetails.Clear();
            for (int i = 0; i < days.Count; i++)
            {
                var tripDetail = new TripDetail
                {
                    Day = DateOnly.FromDateTime(days[i]),  // Single day for each TripDetail
                    MainTopic = mainTopics[i],
                    SubTopic = subTopics[i],
                    NotePrice = notePrices[i], // NotePrice is now a string
                    TripId = booking.Trip.TripId  // Assign the TripId to the TripDetail
                };
                booking.Trip.TripDetails.Add(tripDetail);
            }

            // Step 5: Save changes to the database
            _db.SaveChanges();

            // Step 6: Redirect to the quote view after update
            return Redirect("Quote?Bookingid=" + bookingId);
        }

        // ------------------ Sales Staff Functionality Merged Here ---------------------

        // Display the Sales Staff page for managing booking
        public IActionResult Sale(int id)
        {
            // Fetch the booking by ID
            var booking = _db.Bookings
               .Include(b => b.Trip) // Assuming Trip is a navigation property in Booking
               .FirstOrDefault(b => b.BookingId == id);
            
            if (booking == null)
            {
                return NotFound(); // Return 404 if booking not found
            }

            return View(booking); // Pass the booking data to the view
        }

        // Handle the post request to update the booking status for Sales Staff
        [HttpPost]
        public IActionResult UpdateStatusSalesStaff(int id)
        {
            // Fetch the booking by ID
            var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == id);

            if (booking != null)
            {
                // Set the status to 'processing' since that's the only allowed status for sales staff
                booking.Status = "Processing";
                booking.QuoteSentDate = DateOnly.FromDateTime(DateTime.Now);

                // Save the changes
                _db.SaveChanges();
            }

            // Redirect back to the Sales Staff page with updated status
            return Redirect("Sale?id=" + id);
        }
    }
}
