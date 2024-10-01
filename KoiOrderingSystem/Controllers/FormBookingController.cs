using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // For session management
using System.Linq;
using System.Threading.Tasks;

namespace KoiOrderingSystem.Controllers
{
    public class FormBookingController : Controller
    {
        private readonly Koi88Context _db;

        public FormBookingController(Koi88Context db)
        {
            _db = db;
        }

        // GET: FormBooking/Index
        public async Task<IActionResult> Index()
        {
            // Trang này không yêu cầu đăng nhập, ai cũng có thể xem
            return View(await _db.FormBookings.Include(fb => fb.Bookings).ToListAsync());
        }

        // GET: FormBooking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Trang này không yêu cầu đăng nhập, ai cũng có thể xem chi tiết
            var formBooking = await _db.FormBookings
                .Include(fb => fb.Bookings)
                .FirstOrDefaultAsync(m => m.FormBookingId == id);
            if (formBooking == null)
            {
                return NotFound();
            }

            return View(formBooking);
        }

        // GET: FormBooking/Create
        public IActionResult Create()
        {
            // Chỉ cho phép người dùng đã đăng nhập tạo form booking
            if (HttpContext.Session.GetString("Username") == null)
            {
                // Chuyển hướng tới trang đăng nhập nếu chưa đăng nhập
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        // POST: FormBooking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Fullname,Phone,Email,Favoritefarm,EstimatedCost,FavoriteKoi,HotelAccommodation,EstimatedDepartureDate,ReturnDate,Gender,Note")] FormBooking formBooking)
        {
            // Kiểm tra lại nếu người dùng đã đăng nhập chưa
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // Lấy CustomerId từ session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                // Nếu không tìm thấy CustomerId, yêu cầu đăng nhập lại
                return RedirectToAction("Login", "Login");
            }

            if (ModelState.IsValid)
            {
                // Gán CustomerId từ session cho formBooking
                formBooking.CustomerId = customerId.Value;

                // Thêm form booking mới vào database
                _db.Add(formBooking);
                await _db.SaveChangesAsync();

                // Tạo một booking mới liên quan đến form booking
                var newBooking = new Booking
                {
                    FormBookingId = formBooking.FormBookingId,
                    StartDate = formBooking.EstimatedDepartureDate?.ToDateTime(new TimeOnly(0, 0)),
                    EndDate = formBooking.ReturnDate?.ToDateTime(new TimeOnly(0, 0)),
                    Status = "Pending",
                    BookingDate = DateTime.Now // Gán BookingDate tại đây

                };
                _db.Add(newBooking);
                await _db.SaveChangesAsync();

                // Điều hướng sau khi tạo thành công
                return RedirectToAction("HomePage","Home"); // Chuyển hướng đến action Index của FormBooking
            }

            return View(formBooking); // Quay lại form nếu có lỗi xác thực
        }

        // GET: FormBooking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Chỉ cho phép người dùng đã đăng nhập chỉnh sửa form booking
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            var formBooking = await _db.FormBookings.FindAsync(id);
            if (formBooking == null)
            {
                return NotFound();
            }
            return View(formBooking);
        }

        // POST: FormBooking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FormBookingId,Fullname,Phone,Email,Favoritefarm,EstimatedCost,FavoriteKoi,HotelAccommodation,EstimatedDepartureDate,ReturnDate,Gender,Note")] FormBooking formBooking)
        {
            // Chỉ cho phép người dùng đã đăng nhập chỉnh sửa form booking
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (id != formBooking.FormBookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(formBooking);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormBookingExists(formBooking.FormBookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index"); // Sau khi sửa, chuyển hướng về trang Index
            }
            return View(formBooking);
        }

        // GET: FormBooking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Chỉ cho phép người dùng đã đăng nhập xóa form booking
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            var formBooking = await _db.FormBookings
                .FirstOrDefaultAsync(m => m.FormBookingId == id);
            if (formBooking == null)
            {
                return NotFound();
            }

            return View(formBooking);
        }

        // POST: FormBooking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Chỉ cho phép người dùng đã đăng nhập xóa form booking
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var formBooking = await _db.FormBookings.FindAsync(id);
            if (formBooking != null)
            {
                _db.FormBookings.Remove(formBooking);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index"); // Sau khi xóa, chuyển hướng về trang Index
        }

        private bool FormBookingExists(int id)
        {
            return _db.FormBookings.Any(e => e.FormBookingId == id);
        }
    }
}
