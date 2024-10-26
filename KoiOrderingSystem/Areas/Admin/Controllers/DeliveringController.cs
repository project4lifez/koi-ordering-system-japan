using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoiOrderingSystem.Controllers.Admin;
using PayPal.Api;

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
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            // Check if the RoleId is null or not equal to 5
            if (adminRoleId == null || adminRoleId != 5)
            {
                return NotFound("You do not have permission to access this page.");
            }

            // Load Booking with related PO, PODetails and Payment Method
            var booking = _context.Bookings
                                  .Include(b => b.Po)
                                  .ThenInclude(po => po.Podetails)
                                  .ThenInclude(podetail => podetail.Koi)
                                  .Include(b => b.Po)
                                  .ThenInclude(po => po.Popayments)
                                  .ThenInclude(poPayment => poPayment.PaymentMethod)
                                  .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }

            // Truy vấn danh sách PaymentMethods
            var paymentMethods = _context.PaymentMethods.ToList();

            // Tạo một đối tượng ViewBag để truyền dữ liệu vào View
            ViewBag.PaymentMethods = paymentMethods;

            // Pass the booking data to the view
            return View(booking);
        }


        // Action to update status
        [HttpPost]
        public IActionResult UpdateStatusDelivering(int id, string status)
        {
            // Tìm booking theo ID
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == id);

            if (booking != null)
            {
                // Chặn thay đổi trạng thái nếu nó đã là 'Delivered' hoặc 'Failed'
                if (booking.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase) ||
                    booking.Status.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("StatusError", "Cannot update status once it is set to 'Delivered' or 'Failed'.");
                    return View("Delivering", booking);
                }

                // Nếu cập nhật thành 'Delivered' thì booking phải đang là 'Delivering'
                if (status.Equals("Delivered", StringComparison.OrdinalIgnoreCase) &&
                    !booking.Status.Equals("Delivering", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("StatusError", "The booking must be in 'Delivering' status before updating to 'Delivered'.");
                    return View("Delivering", booking);
                }

                // Cập nhật trạng thái mới
                booking.Status = status;

                // Nếu trạng thái là 'Delivered', tạo Feedback
                if (status.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                {
                    var feedback = new Feedback
                    {
                        CustomerId = booking.CustomerId,
                        Status = "Pending"
                    };
                    _context.Feedbacks.Add(feedback);
                    _context.SaveChanges();

                    booking.FeedbackId = feedback.FeedbackId;
                }

                // Lưu thay đổi vào database
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Booking status updated to '{status}' successfully.";
            }

            return Redirect("Delivering?id=" + id);
        }



        [HttpPost]
        public IActionResult SetStatusToPaid(int id)
        {
            // Tìm booking dựa trên bookingId
            var booking = _context.Bookings
                                  .Include(b => b.Po)
                                  .FirstOrDefault(b => b.BookingId == id);

            if (booking != null)
            {
                // Chỉ cho phép cập nhật trạng thái nếu trạng thái hiện tại là "Checked in"
                if (!booking.Status.Equals("Delivering"))
                {
                    // Thêm thông báo lỗi vào ModelState nếu trạng thái không phải là "Checked in"
                    ModelState.AddModelError("StatusError", "The booking must be in 'Delivering' status before updating to 'Paid'.");

                    // Tải lại trang với thông báo lỗi
                    return View("Delivering", booking);
                }

                // Kiểm tra nếu trạng thái hiện tại chưa phải là "Paid"
                if (!booking.Po.Status.Equals("Paid"))
                {
                    // Cập nhật trạng thái thành "Paid"
                    booking.Po.Status = "Paid";

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.SaveChanges();

                    // Đặt thông báo thành công
                    TempData["SuccessMessage"] = $"Po status updated to 'Paid' successfully.";
                }
            }

            // Chuyển hướng người dùng về trang 'Delivering' sau khi cập nhật thành công
            return Redirect("Delivering?id=" + id);
        }

        [HttpPost]
        public IActionResult UpdatePaymentMethod(int id, int paymentMethodId)
        {
            // Lấy Booking với liên kết đến Po và Popayment
            var booking = _context.Bookings
                                  .Include(b => b.Po)
                                  .ThenInclude(po => po.Popayments)
                                  .FirstOrDefault(b => b.BookingId == id);

            if (booking == null || booking.Po == null)
            {
                return NotFound($"Booking with ID {id} not found or missing PO.");
            }

            // Kiểm tra nếu đã có Popayment tồn tại hay chưa
            var popayment = booking.Po.Popayments.FirstOrDefault();

            // Nếu chưa có Popayment, tạo mới
            if (popayment == null)
            {
                popayment = new Popayment
                {
                    PoId = booking.Po.PoId,  // Gán giá trị PoId
                    PaymentMethodId = paymentMethodId,  // Gán giá trị PaymentMethodId
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now)  // Lấy ngày hiện tại
                };
                _context.Popayments.Add(popayment);  // Thêm mới Popayment vào database
            }
            else
            {
                // Nếu Popayment đã tồn tại, cập nhật PaymentMethodId và PaymentDate
                popayment.PaymentMethodId = paymentMethodId;
                popayment.PaymentDate = DateOnly.FromDateTime(DateTime.Now);  // Lấy ngày hiện tại
            }

            // Lưu thay đổi vào database
            _context.SaveChanges();

            // Thông báo thành công
            TempData["SuccessMessage"] = "Payment method updated successfully.";

            // Quay lại trang Delivering
            return Redirect("Delivering?id=" + id);
        }

        [HttpPost]
        public IActionResult UpdateNote(int id, string note)
        {
            // Tìm booking bằng ID
            var booking = _context.Bookings.Include(b => b.Po).FirstOrDefault(b => b.BookingId == id);
            if (booking?.Po != null)
            {
                // Cập nhật Note trong Po
                booking.Po.Note = note;

                // Lưu thay đổi vào database
                _context.SaveChanges();

                // Thông báo thành công
                TempData["SuccessMessage"] = "Note updated successfully.";
            }

            return Redirect("Delivering?id=" + id);
        }



    }

}
