using KoiOrderingSystem.Controllers.Admin;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ConsultingController : BaseController
    {
        private readonly Koi88Context _context;

        public ConsultingController(Koi88Context context)
        {
            _context = context;
        }

        // Hiển thị trang Consulting với thông tin đặt hàng
        public IActionResult Consulting(int id)
        {
            // Lấy Booking bao gồm PO và Podetails
            var booking = _context.Bookings
                .Include(b => b.Po)
                .ThenInclude(po => po.Podetails)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // Cập nhật trạng thái "checked in" và "checked out"
        [HttpPost]
        public IActionResult UpdateStatusConsulting(int id, string status)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == id);

            if (booking != null)
            {
                // Prevent status update if already "Checked out"
                if (!booking.Status.Equals("checked out", StringComparison.OrdinalIgnoreCase))
                {
                    booking.Status = status;
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Consulting?id=" + id);
        }

        // Cập nhật thông tin chi tiết đơn hàng (Order Detail)
        [HttpPost]
        public IActionResult UpdateOrderDetail(int id, DateTime koiDeliveryDate, string koiDeliveryTime, decimal koiPrice, decimal deposit, string location)
        {
            var booking = _context.Bookings
                .Include(b => b.Po)
                .ThenInclude(po => po.Podetails)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking != null && booking.Po != null)
            {
                var poDetails = booking.Po.Podetails.FirstOrDefault();
                if (poDetails != null)
                {
                    // Cập nhật thông tin PO và Podetails
                    booking.Po.KoiDeliveryDate = DateOnly.FromDateTime(koiDeliveryDate); // Convert DateTime to DateOnly
                    booking.Po.KoiDeliveryTime = TimeOnly.Parse(koiDeliveryTime);       // Convert string to TimeOnly
                    poDetails.TotalKoiPrice = koiPrice;
                    poDetails.Deposit = deposit;
                    booking.HotelAccommodation = location;

                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Consulting?id=" +id);
        }
    }
}
