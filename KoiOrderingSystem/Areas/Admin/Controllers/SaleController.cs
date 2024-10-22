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
            // Retrieve the RoleId from the session
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            // Check if the RoleId is null or not equal to 2 or 3
            if (adminRoleId == null || (adminRoleId != 2 && adminRoleId != 3))
            {
                return NotFound("You do not have permission to access this page.");
            }

            var booking = _db.Bookings
                .Include(b => b.Trip)
                .ThenInclude(t => t.TripDetails)
                .ThenInclude(td => td.KoiFarm)
                .FirstOrDefault(b => b.BookingId == bookingId);

            // Check if booking exists
            if (booking == null)
            {
                return NotFound($"Booking with ID {bookingId} not found.");
            }

            return View(booking);
        }



        // Handle the POST request to create quote information with multiple trip details
        [HttpPost]
        {
            // Step 1: Retrieve booking information from the database
            var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

            // Step 2: Check if booking exists
            if (booking == null)
            {
                return NotFound($"Booking with ID {bookingId} not found.");
            }

            // Ensure lists are of the same size
         

            // Step 3: Create a new Trip
            var trip = new Trip
            {
                TripName = tripName,
                TripDetails = new List<TripDetail>()
            };

            // Step 4: Create multiple TripDetails based on the user inputs
            for (int i = 0; i < days.Count; i++)
            {
                // Step 4.1: Create a new TripDetail and associate it with the farm ID directly
                var tripDetail = new TripDetail
                {
                    Day = DateOnly.FromDateTime(days[i]),  // Single day for each TripDetail
                    MainTopic = mainTopics[i],
                    SubTopic = subTopics[i],
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
        }




        // Handle the POST request to update the quote information with multiple trip details
        [HttpPost]
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

            // Ensure lists are of the same size
            if (days.Count != farmIds.Count || days.Count != mainTopics.Count || days.Count != subTopics.Count || days.Count != notePrices.Count)
            {
                return BadRequest("Mismatch between the number of days, farms, main topics, subtopics, and note prices.");
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

            for (int i = 0; i < days.Count; i++)
            {
                // Retrieve the farm using the farmId
                var farm = _db.KoiFarms.FirstOrDefault(f => f.FarmId == farmIds[i]);
                if (farm == null)
                {
                    return NotFound($"Farm with ID {farmIds[i]} not found.");
                }

                // Create a new TripDetail and associate it with the farm
                var tripDetail = new TripDetail
                {
                    Day = DateOnly.FromDateTime(days[i]),  // Single day for each TripDetail
                    MainTopic = mainTopics[i],
                    SubTopic = subTopics[i],
                    TripId = booking.Trip.TripId  // Assign the TripId to the TripDetail
                };

                // Add the new TripDetail to the list
                booking.Trip.TripDetails.Add(tripDetail);
            }

            _db.SaveChanges();

            return Redirect("Quote?Bookingid=" + bookingId);
        }





        // ------------------ Sales Staff Functionality Merged Here ---------------------

        // Display the Sales Staff page for managing booking
        public IActionResult Sale(int id)
        {
            // Retrieve the RoleId from the session
            var roleId = HttpContext.Session.GetInt32("AdminRoleId");

            // Check if the RoleId is null or not equal to 3
            if (roleId == null || roleId != 3)
            {
                return NotFound("You do not have permission to access this page.");
            }

            // Fetch the booking by ID
            var booking = _db.Bookings
                .Include(b => b.Trip) // Assuming Trip is a navigation property in Booking
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }

            // Pass the booking data to the view
            return View(booking);
        }


        // Handle the post request to update the booking status for Sales Staff
        [HttpPost]
        public IActionResult UpdateStatusSalesStaff(int id)
        {
            // Fetch the booking by ID
            var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == id);

            if (booking != null)
            {
                // Set the status to 'Processing' since that's the only allowed status for sales staff
                booking.Status = "Processing";
                booking.QuoteSentDate = DateOnly.FromDateTime(DateTime.Now);

                // Save the changes
                _db.SaveChanges();

                // Set a success message
                TempData["SuccessMessage"] = "Status updated to 'Processing' successfully.";
            }

            // Redirect back to the Sales Staff page with updated status
            return Redirect("Sale?id=" + id);
        }

    }
}
