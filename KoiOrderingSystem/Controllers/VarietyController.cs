using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;

namespace KoiOrderingSystem.Controllers
{
    public class VarietyController : Controller
    {

        private readonly Koi88Context _db;

        public VarietyController(Koi88Context db)
        {
            _db = db;
        }

        private async Task<(List<Variety>, int, int)> GetPagedVarieties(IQueryable<Variety> query, int page, int pageSize)
        {
            // Tính tổng số lượng giống cá
            int totalVarieties = await query.CountAsync();

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling(totalVarieties / (double)pageSize);

            // Lấy danh sách giống cá cho trang hiện tại
            var varietiesOnPage = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(v => v.SpecialVarieties)
                    .ThenInclude(sv => sv.Farm)
                .ToListAsync();

            return (varietiesOnPage, totalPages, totalVarieties);
        }

        public async Task<IActionResult> Variety(int page = 1)
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != null && adminRoleId >= 2 && adminRoleId <= 5)
            {
                return RedirectToAction("Home", "Admin");
            }
            int pageSize = 8;

            // Query lấy tất cả các varieties
            var query = _db.Varieties.AsQueryable();

            // Gọi phương thức chung để lấy danh sách phân trang
            var (varietiesOnPage, totalPages, totalVarieties) = await GetPagedVarieties(query, page, pageSize);

            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(varietiesOnPage);
        }


        public async Task<IActionResult> Search(string title, string variety, string breeder, int page = 1)
        {
            int pageSize = 8;

            // Tạo query cơ bản
            var query = _db.Varieties.AsQueryable();

            // Tìm kiếm theo tiêu đề (VarietyName)
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(v => v.VarietyName.Contains(title));
            }

            // Nếu variety không được cung cấp, không cần lọc theo variety
            if (!string.IsNullOrEmpty(variety))
            {
                query = query.Where(v => v.VarietyName == variety);
            }

            // Tìm kiếm theo breeder
            if (!string.IsNullOrEmpty(breeder))
            {
                query = query.Where(v => v.SpecialVarieties
                    .Any(sv => sv.Farm.FarmName.Contains(breeder)));
            }

            // Gọi phương thức chung để lấy danh sách phân trang
            var (varietiesOnPage, totalPages, totalVarieties) = await GetPagedVarieties(query, page, pageSize);

            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View("Variety", varietiesOnPage);
        }

        public async Task<IActionResult> VarietyDetail(int id)
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != null && adminRoleId >= 2 && adminRoleId <= 5)
            {
                return RedirectToAction("Home", "Admin");
            }

            var variety = await _db.Varieties
                .Include(v => v.KoiFishes)
                .FirstOrDefaultAsync(v => v.VarietyId == id);

            if (variety == null)
            {
                return NotFound();
            }

            return View(variety);
        }

        [HttpGet]
        public async Task<IActionResult> GetKoiDetail(int id)
        {
            var koi = await _db.KoiFishes
                .Include(k => k.Variety)
                .ThenInclude(v => v.SpecialVarieties)
                .ThenInclude(sv => sv.Farm)
                .FirstOrDefaultAsync(k => k.KoiId == id);

            if (koi == null)
            {
                return NotFound();
            }

            var result = new
            {
                ImageUrl = koi.ImageUrl,
                KoiName = koi.KoiName,
                Description = koi.Description,
                Price = koi.Price,
                KoiNameJp = koi.Koinamejp,
                Age = koi.Age,
                Size = koi.Size,
                Variety = koi.Variety?.VarietyName,
                Farm = koi.Variety?.SpecialVarieties
                    .Select(sv => sv.Farm?.FarmName) // Lấy danh sách các tên farm
                    .Where(farmName => farmName != null) // Loại bỏ các farm null
                    .ToList()
            };

            return Json(result);
        }


    }
}
