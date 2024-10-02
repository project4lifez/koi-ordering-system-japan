using KoiOrderingSystem.Controllers.Admin;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
	[Area("Admin")]

	public class SaleController : BaseController
	{
		private readonly Koi88Context _db;
		public IActionResult Sale()
		{
			return View();
		}

		public SaleController(Koi88Context db)
		{
			_db = db;
		}
		public IActionResult Quote(int bookingId)
		{

			// Lấy thông tin booking từ database
			var booking = _db.Bookings
				.FirstOrDefault(b => b.BookingId == bookingId);

			if (booking == null)
			{
				return NotFound(); // Nếu không tìm thấy booking
			}

			// Lấy customerId từ booking
			var customerId = booking.CustomerId;

			// Lấy thông tin khách hàng từ database
			var customer = _db.Customers
				.FirstOrDefault(c => c.CustomerId == customerId);

			if (customer == null)
			{
				return NotFound(); 
			}

          
            ViewBag.BookingId = booking.BookingId;
            ViewBag.CustomerId = customer.CustomerId;
            ViewBag.CustomerName = booking.Fullname; // Nếu khách hàng có tên trong Account
            ViewBag.BookingDate = booking.BookingDate?.ToString("yyyy-MM-dd"); // Định dạng ngày tháng
            ViewBag.TotalAmount = booking.QuotedAmount;
			ViewBag.StartDate = booking.StartDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = booking.EndDate?.ToString("yyyy-MM-dd");


            return View(booking); 
		}

        [HttpPost] // Chỉ định đây là một phương thức POST
        public IActionResult UpdateDates(int bookingId, DateTime startDate, DateTime endDate)
        {
            // Lấy thông tin booking từ database
            var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound(); // Nếu không tìm thấy booking
            }

            // Cập nhật StartDate và EndDate
            booking.StartDate = DateOnly.FromDateTime(startDate); // Chuyển đổi sang DateOnly
            booking.EndDate = DateOnly.FromDateTime(endDate); // Chuyển đổi sang DateOnly

            // Lưu thay đổi vào cơ sở dữ liệu
            _db.SaveChanges();

            return RedirectToAction("Quote", new { bookingId }); // Chuyển hướng trở lại trang chi tiết
        }
    }
}

