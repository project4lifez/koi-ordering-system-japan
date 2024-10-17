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
            var booking = _db.Bookings
                             .Include(b => b.Po)
                             .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                ModelState.AddModelError(string.Empty, "Booking not found.");
                return View("Consulting", booking);
            }

            if (status == "checkout" && booking.Status != "Checked in")
            {
                ModelState.AddModelError("StatusError", "Cannot check out without checking in first.");
                return View("Consulting", booking);
            }

            if (status == "checkin")
            {
                booking.Status = "Checked in";

                // Nếu không có Po, tạo mới Po và gán trạng thái "Created"
                if (booking.Po == null)
                {
                    booking.Po = new Po
                    {
                        Status = "Created"
                        // Các thuộc tính khác nếu có thể cần được khởi tạo
                    };
                    _db.Add(booking.Po);
                }
                else
                {
                    booking.Po.Status = "Created"; // Cập nhật Po nếu đã tồn tại
                }
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
        public IActionResult UpdateDelivering(int bookingId, DateOnly deliveryDate, TimeOnly deliveryTime, string deliveryLocation)
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

            // Update or create Po with new delivery information, but without updating koiPrice or deposit
            if (booking.Po == null)
            {
                booking.Po = new Po
                {
                    KoiDeliveryDate = deliveryDate,
                    KoiDeliveryTime = deliveryTime,
                    DeliveryLocation = deliveryLocation
                };

                _db.Add(booking.Po);
            }
            else
            {
                booking.Po.KoiDeliveryDate = deliveryDate;
                booking.Po.KoiDeliveryTime = deliveryTime;
                booking.Po.DeliveryLocation = deliveryLocation;
            }

            // Save changes to the database
            _db.SaveChanges();

            return Redirect("Consulting?Bookingid=" + bookingId);
        }

        [HttpPost]
        public IActionResult SetStatusToDeposited(int bookingId, int poId)
        {
            // Tìm booking dựa trên bookingId và bao gồm cả thông tin Po
            var booking = _db.Bookings
                             .Include(b => b.Po)
                             .FirstOrDefault(b => b.BookingId == bookingId && b.Po.PoId == poId);

            // Nếu không tìm thấy booking hoặc Po, trả về lỗi
            if (booking == null || booking.Po == null)
            {
                ModelState.AddModelError(string.Empty, "Booking or Po not found.");
                return View("Consulting", booking);
            }

            // Cập nhật trạng thái của Po thành "Deposited"
            booking.Po.Status = "Deposited";

            // Lưu thay đổi vào cơ sở dữ liệu
            _db.SaveChanges();

            // Redirect về trang Consulting với thông báo thành công
            TempData["SuccessMessage"] = "Po Status has been updated to 'Deposited'.";
            return Redirect("Consulting?BookingId=" + bookingId);
        }

    }
}
