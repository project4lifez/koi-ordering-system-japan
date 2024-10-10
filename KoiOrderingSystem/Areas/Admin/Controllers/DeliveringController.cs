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
            var roleId = HttpContext.Session.GetInt32("RoleId");

            // Check if the RoleId is null or not equal to 5
            if (roleId == null || roleId != 5)
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
            // Tìm kiếm booking theo ID
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == id);

            if (booking != null)
            {
                // Ngăn việc thay đổi trạng thái nếu trạng thái chưa phải là 'Delivering' và người dùng cố gắng thay đổi thành 'Delivered'
                if (status.Equals("Delivered", StringComparison.OrdinalIgnoreCase) &&
                    !booking.Status.Equals("Delivering", StringComparison.OrdinalIgnoreCase))
                {
                    // Add an error message to ModelState if trying to set 'Delivered' without 'Delivering'
                    ModelState.AddModelError("StatusError", "The booking must be in 'Delivering' status before updating to 'Delivered'.");

                    // Reload the view with the error message
                    return View("Delivering", booking);
                }

                // Ngăn việc thay đổi trạng thái nếu trạng thái đã là 'Delivered'
                if (!booking.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                {
                    // Cập nhật trạng thái mới
                    booking.Status = status;

                    // Lưu thay đổi vào database
                    _context.SaveChanges();
                }
            }

            // Chuyển hướng về trang 'Delivering' sau khi cập nhật thành công
            return Redirect("Delivering?id=" + id);
        }
    }
}
