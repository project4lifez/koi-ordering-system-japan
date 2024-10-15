using KoiOrderingSystem.Controllers.Admin;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ConsultingController : BaseController
    {
        private readonly Koi88Context _db;

        public ConsultingController(Koi88Context db)
        {
            _db = db;
        }

        // Display the Consulting Staff Dashboard
        public IActionResult Consulting(int bookingId)
        {
            // Retrieve the RoleId from the session
            var roleId = HttpContext.Session.GetInt32("AdminRoleId");

            // Check if the RoleId is null or not equal to 4
            if (roleId == null || roleId != 4)
            {
                return NotFound("You do not have permission to access this page.");
            }

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
                ModelState.AddModelError(string.Empty, "Booking not found.");
                return View("Consulting", booking);
            }

            // Ensure that "Checked out" can only happen if the booking is already "Checked in"
            if (status == "checkout" && booking.Status != "Checked in")
            {
                ModelState.AddModelError("StatusError", "Cannot check out without checking in first.");
                return View("Consulting", booking); // Return view with error
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

            TempData["SuccessMessage"] = $"Booking status updated to '{booking.Status}' successfully.";

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
                ModelState.AddModelError(string.Empty, "Booking not found.");
                return View("Consulting", booking);
            }

            // Ensure the status is "Checked in" or "Checked out" before updating delivery details
            if (booking.Status != "Checked in" && booking.Status != "Checked out")
            {
                ModelState.AddModelError("DeliveryError", "UpdateDelivering can only be performed when the status is 'Checked in' or 'Checked out'.");
                return View("Consulting", booking);
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
