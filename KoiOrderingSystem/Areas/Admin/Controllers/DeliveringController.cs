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
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            // Check if the RoleId is null or not equal to 5
            if (adminRoleId == null || adminRoleId != 5)
            {
                return NotFound("You do not have permission to access this page.");
            }

            // Load Booking with related PO and PODetails
            var booking = _context.Bookings
      .Include(b => b.Po)                  // Include the related PO
      .ThenInclude(po => po.Podetails)      // Include the related PODetails
      .ThenInclude(podetail => podetail.Koi)
      .Include(t => t.Trip)// Include the related Koi for each Podetail
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
            // Find the booking by ID
            var booking = _context.Bookings
                                  .FirstOrDefault(b => b.BookingId == id);

            if (booking != null)
            {
                // Prevent changing status if it is not 'Delivering' when trying to set it to 'Delivered'
                if (status.Equals("Delivered", StringComparison.OrdinalIgnoreCase) &&
                    !booking.Status.Equals("Delivering", StringComparison.OrdinalIgnoreCase))
                {
                    // Add an error message to ModelState if trying to set 'Delivered' without 'Delivering'
                    ModelState.AddModelError("StatusError", "The booking must be in 'Delivering' status before updating to 'Delivered'.");

                    // Reload the view with the error message
                    return View("Delivering", booking);
                }

                // Prevent changing status if it is already 'Delivered'
                if (!booking.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                {
                    // Update the new status
                    booking.Status = status;

                    // If status is set to 'Delivered', create a new Feedback record
                    if (status.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                    {
                        // Create a new Feedback object
                        var feedback = new Feedback
                        {
                            CustomerId = booking.CustomerId, // Assign the CustomerId from the Booking
                            Status = "Pending" // Set Feedback status to 'Pending'
                        };

                        // Add the new Feedback to the context
                        _context.Feedbacks.Add(feedback);
                        _context.SaveChanges(); // Save to get the new Feedback ID

                        // Now associate the newly created Feedback ID with the Booking
                        booking.FeedbackId = feedback.FeedbackId; // Assuming Booking has a FeedbackId property
                    }

                    // Save changes to the database
                    _context.SaveChanges();

                    // Set success message
                    TempData["SuccessMessage"] = $"Booking status updated to '{status}' successfully.";
                }
            }

            // Redirect back to the 'Delivering' page after successful update
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


    }

}
