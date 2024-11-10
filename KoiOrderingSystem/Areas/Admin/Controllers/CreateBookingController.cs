using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CreateBookingController : Controller
    {
        private readonly Koi88Context _db;

        public CreateBookingController(Koi88Context db)
        {
            _db = db;
        }

        // GET: Admin/CreateBooking/Create
        public IActionResult Create()
        {
            // Load Customer list
            ViewBag.Customers = _db.Customers
                .Include(c => c.Account)
                .Select(c => new
                {
                    c.CustomerId,
                    FullName = $"{c.Account.Lastname} {c.Account.Firstname}",
                    Email = c.Account.Email
                })
                .ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookingViewModel model)
        {
            model.CustomerId = 6;

            // Gán cứng CustomerId

            // Lấy thông tin khách hàng dựa trên CustomerId gán cứng
            var customer = await _db.Customers
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.CustomerId == model.CustomerId);

            if (customer == null)
            {
                ModelState.AddModelError("", "Customer does not exist.");
                return View(model);
            }

            // Tạo booking mới
            var booking = new Booking
            {
                CustomerId = customer.CustomerId,
                Fullname = $"{customer.Account.Lastname} {customer.Account.Firstname}",
                StartDate = DateOnly.FromDateTime(model.StartDate),
                EndDate = DateOnly.FromDateTime(model.EndDate),
                Email = customer.Account.Email,
                Phone = customer.Account.Phone,
                BookingDate = DateOnly.FromDateTime(DateTime.Now),
                Status = "Requested",
                IsActive = true
            };

            // Lưu booking vào cơ sở dữ liệu
            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            return Redirect("/Admin/Home/OrderManagement");
        }



        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _db.Customers
     .Include(c => c.Account)
     .Where(c => c.Account != null)  // Only include customers with an account
     .Select(c => new
     {
         CustomerId = c.CustomerId,
         FullName = $"{c.Account.Lastname} {c.Account.Firstname}",
         Phone = c.Account.Phone,
         Email = c.Account.Email
     })
     .ToListAsync();

            return Json(customers);
        }

    }
}
