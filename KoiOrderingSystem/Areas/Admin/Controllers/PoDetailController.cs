using KoiOrderingSystem.Controllers.Admin;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class PoDetailController : BaseController
    {
        private readonly Koi88Context _db;

        public PoDetailController(Koi88Context db)
        {
            _db = db;
        }

        public IActionResult PoDetail(int bookingId)
        {
            // Truy xuất thông tin booking với tất cả các quan hệ cần thiết
            var booking = _db.Bookings
                .Include(b => b.Po)
                    .ThenInclude(p => p.Podetails)
                        .ThenInclude(pd => pd.Koi)
                            .ThenInclude(kf => kf.Variety)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.TripDetails)
                        .ThenInclude(td => td.KoiFarm)
                .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            ViewBag.Booking = booking;

            // Truy xuất danh sách farm, bao gồm các SpecialVariety và Variety để lấy Koi Fish
            var farms = _db.KoiFarms
                .Include(f => f.SpecialVarieties)  // Bao gồm Special Varieties của farm
                    .ThenInclude(sv => sv.Variety)  // Từ Special Variety lấy Variety
                        .ThenInclude(v => v.KoiFishes)  // Từ Variety lấy Koi Fish
                .ToList();

            ViewBag.Farms = farms;

            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePoOrder(int bookingId, [FromForm] List<PoDetailInputModel> poDetails)
        {
            var booking = await _db.Bookings
                .Include(b => b.Po)
                .ThenInclude(p => p.Podetails)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            // Xóa tất cả PoDetails cũ cho booking này để chuẩn bị thêm các PoDetails mới
            _db.Podetails.RemoveRange(booking.Po.Podetails);

            // Duyệt qua tất cả các chi tiết của từng ngày (poDetails)
            foreach (var poDetail in poDetails)
            {
                // Xử lý từng ngày, với mỗi ngày có thể có nhiều con cá
                for (int i = 0; i < poDetail.KoiId.Length; i++)
                {
                    var newPoDetail = new Podetail
                    {
                        PoId = booking.Po.PoId,
                        KoiId = poDetail.KoiId[i],
                        FarmId = poDetail.FarmId,
                        Deposit = poDetail.Deposit,
                        Day = poDetail.Day,
                        Quantity = poDetail.Quantity[i],
                        TotalKoiPrice = poDetail.TotalKoiPrice[i],
                        Note = poDetail.Note
                    };

                    // Thêm PoDetail mới cho ngày đó và con cá đó
                    _db.Podetails.Add(newPoDetail);
                }
            }

            // Tính tổng giá trị cho đơn hàng
            var totalKoiPrice = poDetails.Sum(detail => detail.TotalKoiPrice.Sum() ?? 0);
            var totalDeposit = poDetails.Sum(detail => detail.Deposit ?? 0);
            booking.Po.TotalAmount = totalKoiPrice - totalDeposit;

            await _db.SaveChangesAsync();

            return Redirect("PoDetail/PoDetail?bookingId=" + bookingId);
        }
    }
}











    

