using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
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
			var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == id);
			if (booking == null)
			{
				return NotFound();
			}

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
                // Ngăn việc thay đổi trạng thái nếu trạng thái đã là 'delivered'
                if (!booking.Status.Equals("delivered", StringComparison.OrdinalIgnoreCase))
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
