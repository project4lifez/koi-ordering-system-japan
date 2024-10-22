﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Http; // For session management
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PayPal.Api;
using System.Globalization; // For formatting currency
using Microsoft.Extensions.Configuration; // To access app settings

namespace KoiOrderingSystem.Controllers
{
    public class YourBookingController : Controller
    {
        private readonly Koi88Context _db;
        private readonly IConfiguration _configuration;

        public YourBookingController(Koi88Context db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        // Action to display the list of bookings
        public async Task<IActionResult> YourBooking(string searchKeyword, int page = 1)
        {
            // Define the number of records per page
            int pageSize = 10;

            // Check if the user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("", "Login");
            }

            // Get CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("", "Login");
            }

            // Get the list of bookings for the current user
            var bookingsQuery = _db.Bookings
                .Include(b => b.Trip)        // Include the related Trip entity to access TripName
                .Include(b => b.Feedback)    // Include the related Feedback entity
                .Where(b => b.CustomerId == customerId.Value &&
                            b.Status != "Canceled" &&
                            (b.Feedback == null || b.Feedback.Status != "Completed"));

            // Nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                searchKeyword = searchKeyword.ToLower();

                DateOnly? parsedDate = null;

                // Cố gắng chuyển từ khóa thành ngày theo format M/d/yyyy
                if (DateOnly.TryParseExact(searchKeyword, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly dateValue))
                {
                    parsedDate = dateValue;
                }

                // Áp dụng bộ lọc tìm kiếm cho tên chuyến đi, trạng thái, và ngày
                bookingsQuery = bookingsQuery.Where(b =>
                    (b.Trip != null && b.Trip.TripName.ToLower().Contains(searchKeyword)) || // Tìm kiếm trong Trip Name
                    b.Status.ToLower().Contains(searchKeyword) || // Tìm kiếm trong Status
                    (parsedDate != null && b.BookingDate == parsedDate.Value)); // Tìm kiếm trong Booking Date
            }

            // Đếm tổng số bản ghi
            int totalRecords = await bookingsQuery.CountAsync();

            // Lấy bản ghi cho trang hiện tại
            var bookings = await bookingsQuery
                .Skip((page - 1) * pageSize) // Bỏ qua các trang trước đó
                .Take(pageSize)              // Chỉ lấy số lượng bản ghi của trang hiện tại
                .ToListAsync();

            // Sử dụng ViewBag để truyền dữ liệu phân trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.SearchKeyword = searchKeyword;

            return View(bookings); // Trả dữ liệu bookings và phân trang về view
        }


      




        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(int bookingId, int rating, string comments)
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("", "Login");
            }

            // Lấy CustomerId từ session để kiểm tra người dùng hiện tại
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("", "Login");
            }

            // Tìm booking dựa trên bookingId và kiểm tra tồn tại
            var booking = await _db.Bookings
                            .Include(b => b.Feedback)
                            .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.CustomerId == customerId.Value);

            // Nếu không tìm thấy booking hoặc không có quyền (CustomerId không khớp), trả về lỗi 404
            if (booking == null || booking.Feedback == null)
            {
                return NotFound("Booking not found or you don't have permission to update this feedback.");
            }

            // Cập nhật Feedback nếu booking thuộc về đúng khách hàng
            booking.Feedback.Rating = rating;
            booking.Feedback.Comments = comments;
            booking.Feedback.Status = "Completed"; // Cập nhật trạng thái feedback thành 'Completed'
            booking.Feedback.Feedbackdate = DateOnly.FromDateTime(DateTime.Now);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _db.SaveChangesAsync();

            // Điều hướng lại về trang "YourBooking"
            return RedirectToAction("YourBooking");
        }

        // Payment Action to display the payment form for a specific booking
        [Authorize] // Ensure the user is logged in
        public async Task<IActionResult> Payment(int bookingId)
        {
            // Fetch CustomerId from the session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                // If CustomerId is not found in the session, redirect to login
                return RedirectToAction("Login", "Account");
            }

            // Fetch the booking that belongs to the customer
            var booking = await _db.Bookings
                .Include(b => b.Trip) // Include related Trip entity
                .Include(b => b.BookingPayments) // Include BookingPayments
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.CustomerId == customerId.Value);

            if (booking == null)
            {
                // If no matching booking is found, return 404 or unauthorized
                return Unauthorized("You are not authorized to view this booking.");
            }

            // Pass the booking to the Payment view
            return View(booking);
        }

        // Action to handle form postback from the payment form
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(int bookingId)
        {
            // Check if the user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Find the specific booking for the customer
            var booking = await _db.Bookings
                .Include(b => b.Trip)
                .Include(b => b.BookingPayments)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.CustomerId == customerId.Value);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Store bookingId in session
            HttpContext.Session.SetInt32("BookingId", bookingId);

            // Define tax amount (5%)
            decimal taxAmount = (decimal)booking.QuotedAmount * 0.05m; // Tax as 5% of the quoted amount

            // Create PayPal API context with credentials
            var apiContext = new APIContext(new OAuthTokenCredential(
                _configuration["PayPal:ClientId"],
                _configuration["PayPal:ClientSecret"]).GetAccessToken());

            apiContext.Config = new Dictionary<string, string>
    {
        { "mode", _configuration["PayPal:Mode"] } // Set to sandbox or live
    };

            // Create a PayPal payment object
            var payment = new Payment()
            {
                intent = "sale",
                payer = new Payer() { payment_method = "paypal" },
                redirect_urls = new RedirectUrls()
                {
                    cancel_url = Url.Action("PaymentCancelled", "YourBooking", null, Request.Scheme),
                    return_url = Url.Action("PaymentExecuted", "YourBooking", null, Request.Scheme)
                },
                transactions = new List<Transaction>
        {
            new Transaction
            {
                description = $"Payment for Booking #{booking.BookingId} - Trip: {booking.Trip.TripName}, Dates: {booking.StartDate:yyyy-MM-dd} to {booking.EndDate:yyyy-MM-dd}",
                invoice_number = Guid.NewGuid().ToString(), // Unique invoice number
                amount = new Amount
                {
                    currency = "USD", // Use the appropriate currency
                    total = ((decimal)booking.QuotedAmount + taxAmount).ToString("F2"), // Total includes quoted amount and tax
                    details = new Details
                    {
                        subtotal = ((decimal)booking.QuotedAmount).ToString("F2"),
                        tax = taxAmount.ToString("F2") // Tax amount
                    }
                },
                item_list = new ItemList
                {
                    items = new List<Item>
                    {
                        new Item
                        {
                            name = booking.Trip.TripName, // Name of the trip
                            currency = "USD",
                            price = ((decimal)booking.QuotedAmount).ToString("F2"), // Price for the trip
                            quantity = "1" // Assuming one booking per transaction
                        }
                    }
                }
            }
        }
            };

            // Create the payment in PayPal
            var createdPayment = payment.Create(apiContext);

            // Get the approval URL and redirect the user to PayPal for payment
            var approvalUrl = createdPayment.links.FirstOrDefault(link => link.rel.ToLower() == "approval_url")?.href;
            if (approvalUrl != null)
            {
                return Redirect(approvalUrl);
            }

            return BadRequest("Unable to create payment.");
        }






        // After PayPal returns with payment approval

        public async Task<IActionResult> PaymentExecuted(string paymentId, string token, string PayerID)
        {
            // Get PayPal API context with credentials
            var apiContext = new APIContext(new OAuthTokenCredential(
                _configuration["PayPal:ClientId"],
                _configuration["PayPal:ClientSecret"]).GetAccessToken());

            apiContext.Config = new Dictionary<string, string>
    {
        { "mode", _configuration["PayPal:Mode"] }
    };

            // Lấy bookingId từ session
            var bookingId = HttpContext.Session.GetInt32("BookingId");
            if (bookingId == null)
            {
                return NotFound("Booking ID not found in session.");
            }

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("", "Login");
            }

            try
            {
                // Execute the PayPal payment
                var paymentExecution = new PaymentExecution { payer_id = PayerID };
                var payment = new Payment { id = paymentId };
                var executedPayment = payment.Execute(apiContext, paymentExecution);

                // Check if payment was successful
                if (executedPayment.state.ToLower() == "approved")
                {
                    // Update the booking and payment information in the database
                    var booking = await _db.Bookings
                        .Include(b => b.Trip) // Include related Trip entity
                        .Include(b => b.BookingPayments) // Include BookingPayments
                        .FirstOrDefaultAsync(b => b.BookingId == bookingId.Value && b.CustomerId == customerId.Value);

                    if (booking == null)
                    {
                        return NotFound("Booking not found.");
                    }

                    // Check for an existing BookingPayment
                    var existingPayment = booking.BookingPayments.FirstOrDefault();

                    if (existingPayment != null)
                    {
                        // Update the existing payment entry
                        existingPayment.Status = "Completed";
                        existingPayment.PaymentDate = DateTime.UtcNow;
                        existingPayment.PaymentMethodId = 1; // Assuming PayPal is ID 1

                        // Assign the existing BookingPaymentId to the booking
                        booking.BookingPaymentId = existingPayment.BookingPaymentId;
                    }
                    else
                    {
                        // Create a new BookingPayment entry
                        var bookingPayment = new BookingPayment
                        {
                            Status = "Completed",
                            PaymentDate = DateTime.UtcNow,
                            BookingId = booking.BookingId,
                            PaymentMethodId = 1 // Assuming PayPal is ID 1
                        };

                        // Add the new payment entry to the database
                        await _db.BookingPayments.AddAsync(bookingPayment);

                        // Save changes to retrieve the new BookingPaymentId
                        await _db.SaveChangesAsync();

                        // Assign the new BookingPaymentId to the booking
                        booking.BookingPaymentId = bookingPayment.BookingPaymentId;
                    }

                    // Update the booking status
                    booking.Status = "Confirmed";

                    // Save all changes to the Booking entity
                    await _db.SaveChangesAsync();

                    HttpContext.Session.Remove("BookingId");


                    // Redirect to success or booking details
                    return RedirectToAction("YourBooking");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (using a logging framework) for further diagnosis
                Console.WriteLine(ex.Message); // Replace with proper logging
                return RedirectToAction("PaymentCancelled");
            }

            return RedirectToAction("PaymentCancelled");
        }


        // Payment cancellation handler (nếu canceled thì)
        public IActionResult PaymentCancelled()
        {
            // Get CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get BookingId from session
            var bookingId = HttpContext.Session.GetInt32("BookingId");
            if (bookingId == null)
            {
                return NotFound("Booking ID not found in session.");
            }

            // Retrieve the booking from the database
            var booking = _db.Bookings
                .Include(b => b.BookingPayments) // Include BookingPayments to access the payment
                .FirstOrDefault(b => b.BookingId == bookingId.Value && b.CustomerId == customerId.Value);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Find the latest BookingPayment (you might want to adjust this if there are multiple payments)
            var bookingPayment = booking.BookingPayments.FirstOrDefault();
            if (bookingPayment != null)
            {
                // Update the BookingPayment status to "Fail"
                bookingPayment.Status = "Fail";

                // Save changes to the database
                _db.SaveChanges();
            }
            else
            {
                // Handle case where no BookingPayment exists
                // Optionally, you can create a new BookingPayment with status "Fail" if needed
                var newPayment = new BookingPayment
                {
                    Status = "Fail",
                    PaymentDate = DateTime.UtcNow,
                    BookingId = booking.BookingId,
                    PaymentMethodId = 1 // Assuming a default method ID; adjust as necessary
                };

                // Add the new payment entry to the database
                _db.BookingPayments.Add(newPayment);
                _db.SaveChanges();
            }
            HttpContext.Session.Remove("BookingId");


            // Return a view to inform the user of payment cancellation
            return View("PaymentCancelled");
        }



        // Other actions...







        // Action to resend quote request
        [HttpPost]
        public async Task<IActionResult> ResendQuote(int bookingId)
        {
            // Check if the user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Find the specific booking
            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId && b.CustomerId == customerId.Value);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Update status to "Requested"
            booking.Status = "Requested";
            await _db.SaveChangesAsync();

            return RedirectToAction("YourBooking");
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            // Check if the user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("", "Login");
            }

            // Get CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("", "Login");
            }

            // Find the specific booking
            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId && b.CustomerId == customerId.Value);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Update status to "Requested"
            booking.Status = "Canceled";
            await _db.SaveChangesAsync();

            return RedirectToAction("YourBooking");
        }
        public async Task<IActionResult> BookingHistory(string searchKeywords, int page = 1)
        {
            // Define the number of records per page
            int pageSize = 10;

            // Check if the user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("", "Login");
            }

            // Get CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("", "Login");
            }

           
            // Get the list of bookings for the current user
            var orderHistory = _db.Bookings
                .Include(b => b.Trip)        // Include the related Trip entity so you can access TripName
                .Include(b => b.Feedback)    // Include the related Feedback entity
                .Where(b => b.CustomerId == customerId.Value &&
                            (b.Status == "Canceled" ||
                             (b.Status == "Delivered" && (b.Feedback == null || b.Feedback.Status == "Completed"))));


            // If search keywords are provided
            if (!string.IsNullOrEmpty(searchKeywords))
            {
                searchKeywords = searchKeywords.ToLower();

                DateOnly? parsedDate = null;

                // Try to parse the search keyword as a date (MM/dd/yyyy format)
                if (DateOnly.TryParseExact(searchKeywords, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly dateValue))
                {
                    parsedDate = dateValue;
                }

                // Áp dụng bộ lọc tìm kiếm cho tên chuyến đi, trạng thái, và ngày
                orderHistory = orderHistory.Where(b =>
                    (b.Trip != null && b.Trip.TripName.ToLower().Contains(searchKeywords)) || // Tìm kiếm trong Trip Name
                    b.Status.ToLower().Contains(searchKeywords) || // Tìm kiếm trong Status
                    (parsedDate != null && b.BookingDate == parsedDate.Value)); // Tìm kiếm trong Booking Date
            }



            // Count total records
            int totalRecords = await orderHistory.CountAsync();

            // Get records for the current page
            var bookings = await orderHistory
                .Skip((page - 1) * pageSize) // Skip the previous pages
                .Take(pageSize)              // Take only the number of records for this page
                .ToListAsync();

            // Use ViewBag to pass pagination data
            ViewBag.CurrentPages = page;
            ViewBag.TotalPagess = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.SearchKeywords = searchKeywords;

            return View(bookings); // Pass the bookings and pagination data to the view
        }


        public async Task<IActionResult> GetFeedback(int bookingId)
        {
            // Lấy booking từ cơ sở dữ liệu dựa trên BookingId
            var booking = await _db.Bookings
                                   .Include(b => b.Feedback) // Bao gồm Feedback liên quan
                                   .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            // Kiểm tra nếu có Feedback tồn tại cho Booking này
            if (booking?.Feedback != null)
            {
                // Trả về thông tin Feedback
                return Json(new
                {
                    success = true,
                    feedback = new
                    {
                        rating = booking.Feedback.Rating,
                        comments = booking.Feedback.Comments
                    }
                });
            }

            // Trả về thông báo không có Feedback
            return Json(new { success = false });
        }

        public IActionResult TripDetail(int bookingId)
        {
            // Lấy customerId từ session (ví dụ: giả sử session lưu customerId là "CustomerId")
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            // Kiểm tra xem session có customerId hay không
            if (customerId == null)
            {
                return Unauthorized("You must be logged in to view this page.");
            }

            // Truy xuất thông tin booking từ database và kiểm tra customerId
            var booking = _db.Bookings
        .Include(b => b.Trip) // Bao gồm thông tin Trip liên kết với booking
        .ThenInclude(t => t.TripDetails) // Bao gồm TripDetails
        .ThenInclude(td => td.KoiFarm) // Bao gồm thông tin KoiFarm
        .Include(b => b.Po) // Bao gồm thông tin Purchase Order (Po)
        .ThenInclude(po => po.Podetails) // Bao gồm Podetails
        .ThenInclude(pd => pd.Koi) // Bao gồm thông tin Fish
        .ThenInclude(f => f.Variety) // Bao gồm thông tin Variety
        .FirstOrDefault(b => b.BookingId == bookingId && b.CustomerId == customerId);

            // Kiểm tra nếu booking không tồn tại hoặc không có chuyến đi liên quan
            if (booking == null || booking.Trip == null)
            {
                return NotFound($"Booking with ID {bookingId} not found or does not belong to you.");
            }

            // Kiểm tra trạng thái booking
            if (
                booking.Status != "Canceled" &&
                booking.Status != "Confirmed" &&
                booking.Status != "Checked in" &&
                booking.Status != "Checked out" &&
                booking.Status != "Delivering" &&
                booking.Status != "Delivered")
            {
                return NotFound("You do not have access to this page because the booking is not in a valid status.");
            }
            var farms = _db.KoiFarms.ToList();
            ViewBag.Farms = farms;

            if (booking.Po?.Podetails != null)
            {
                var poDetails = booking.Po.Podetails.ToList();
                ViewBag.PoDetails = poDetails; // Pass it to the view
            }
          

            // Trả về view với thông tin của Trip
            return View(booking);
        }

    }


}
