using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Http; // For session management
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly Koi88Context _db;

        public BookingController(Koi88Context db)
        {
            _db = db;
        }

        // GET: Booking/Create
        public IActionResult Create()
        {
            // Chỉ cho phép người dùng đã đăng nhập tạo form booking
            if (HttpContext.Session.GetString("Username") == null)
            {
                // Chuyển hướng tới trang đăng nhập nếu chưa đăng nhập
                return RedirectToAction("", "Login");
            }

            return View();
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Fullname,Phone,Email,Gender,Favoritefarm,FavoriteKoi,StartDate,EndDate,EstimatedCost,HotelAccommodation,Note")] Booking booking)
        {
            // Kiểm tra lại nếu người dùng đã đăng nhập chưa
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("", "Login");
            }

            // Lấy CustomerId từ session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                // Nếu không tìm thấy CustomerId, yêu cầu đăng nhập lại
                return RedirectToAction("", "Login");
            }

            if (ModelState.IsValid)
            {
                
                
                    // Gán customerId lấy từ session vào booking
                    booking.CustomerId = customerId.Value;
                    booking.BookingDate = DateOnly.FromDateTime(DateTime.Now);
                    booking.Status = "Pending";
                    booking.IsActive = true;

                    // Thêm booking vào database
                    _db.Add(booking);
                    await _db.SaveChangesAsync();

                    // Chuyển hướng đến trang homepage sau khi thành công
                    return RedirectToAction("HomePage", "Home"); // Chuyển hướng về trang homepage (action Index của controller Home)
                


            }

            // Trả về view nếu ModelState không hợp lệ
            return View(booking);
        }
        public async Task<IActionResult> ViewBooking()
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("", "Login");
            }

            // Lấy CustomerId từ session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("", "Login");
            }

            // Lấy danh sách booking của người dùng hiện tại
            var bookings = await _db.Bookings
                                    .Where(b => b.CustomerId == customerId.Value)
                                    .ToListAsync();

            return View(bookings); // Truyền danh sách booking vào view
        }


    }
}