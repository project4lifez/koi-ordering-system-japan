using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ConsultingController : Controller
    {
        private readonly Koi88Context _db;

        public ConsultingController(Koi88Context db)
        {
            _db = db;
        }

        // Display the Consulting Staff Dashboard
        public IActionResult Consulting(int bookingId)
        {
            // Retrieve the booking information from the database, including PO, PODetails, and Trip
            var booking = _db.Bookings
                              .Include(b => b.Po)
                              .ThenInclude(po => po.Podetails)
                              .Include(b => b.Trip)
                              .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound($"Booking with ID {bookingId} not found.");
            }

            // Pass the booking information to the view
            return View(booking);
        }

        // Handle updating Koi Order (PO and PODetail) based on the selected day
        [HttpPost]
        public IActionResult UpdateStatus(int bookingId, string status)
        {
            // Retrieve the booking from the database
            var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound($"Booking with ID {bookingId} not found.");
            }

            // Define restricted statuses where Check-in and Check-out cannot be updated
            var restrictedStatuses = new[] { "Requested", "Rejected", "Processing", "Canceled", "Accepted" };

            // Prevent status update to "Checked In" or "Checked Out" if current status is in the restricted statuses
            if (restrictedStatuses.Contains(booking.Status) && (status == "checkin" || status == "checkout"))
            {
                return BadRequest("Cannot update to 'Check In' or 'Check Out' while the status is in a restricted state.");
            }

            // Update the booking status (only if allowed)
            if (status == "checkin")
            {
                booking.Status = "Checked in";
            }
            else if (status == "checkout")
            {
                booking.Status = "Checked out";
            }

            _db.SaveChanges();

            return Redirect("Consulting?Bookingid=" + bookingId);
        }
        [HttpPost]
        public IActionResult UpdateDelivering(int bookingId, DateOnly deliveryDate, TimeOnly deliveryTime, string deliveryLocation, decimal koiPrice, decimal deposit)
        {
            var booking = _db.Bookings
                             .Include(b => b.Po)
                             .ThenInclude(po => po.Podetails)
                             .Include(b => b.Trip)
                             .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound($"Booking with ID {bookingId} not found.");
            }

            // Kiểm tra trạng thái trước khi cập nhật
            if (booking.Status != "Checked in" && booking.Status != "Checked out")
            {
                return BadRequest("UpdateDelivering can only be performed when the status is 'Checked in' or 'Checked out'.");
            }

            if (booking.Po == null)
            {
                booking.Po = new Po
                {
                    KoiDeliveryDate = deliveryDate,
                    KoiDeliveryTime = deliveryTime,
                    DeliveryLocation = deliveryLocation,
                    TotalAmount = koiPrice,
                };

                _db.Add(booking.Po);
            }
            else
            {
                booking.Po.KoiDeliveryDate = deliveryDate;
                booking.Po.KoiDeliveryTime = deliveryTime;
                booking.Po.DeliveryLocation = deliveryLocation;
                booking.Po.TotalAmount = koiPrice;
            }

            var poDetail = booking.Po.Podetails.FirstOrDefault();
            if (poDetail != null)
            {
                poDetail.Deposit = deposit;
            }
            else
            {
                booking.Po.Podetails.Add(new Podetail
                {
                    Deposit = deposit,
                });
            }

            _db.SaveChanges();
            return Redirect("Consulting?Bookingid=" + bookingId);
        }

    }
}