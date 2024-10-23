using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Controllers
{
    public class FarmController : Controller
    {

        private readonly Koi88Context _db;

        public FarmController(Koi88Context db)
        {
            _db = db;
        }

        private async Task<(List<KoiFarm>, int, int)> GetPagedFarms(IQueryable<KoiFarm> query, int page, int pageSize)
        {
            // Tính tổng số farms
            int totalFarms = await query.CountAsync();

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling(totalFarms / (double)pageSize);

            // Lấy danh sách farms cho trang hiện tại
            var farmsOnPage = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(f => f.SpecialVarieties)
                .ThenInclude(sv => sv.Variety)
                .ToListAsync();

            return (farmsOnPage, totalPages, totalFarms);
        }



        public async Task<IActionResult> Farm(int page = 1)
        {
            int pageSize = 8;

            // Query lấy tất cả các farms
            var query = _db.KoiFarms.AsQueryable();

            // Gọi phương thức chung để lấy danh sách phân trang
            var (farmsOnPage, totalPages, totalFarms) = await GetPagedFarms(query, page, pageSize);

            // Truyền thông tin về trang hiện tại và tổng số trang vào ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(farmsOnPage);
        }


        public async Task<IActionResult> FarmDetail(int id) // Nhận FarmId từ URL
        {
            // Lấy thông tin farm và các giống Koi liên quan
            var farm = await _db.KoiFarms
                .Include(f => f.SpecialVarieties)
                    .ThenInclude(sv => sv.Variety)
                .FirstOrDefaultAsync(f => f.FarmId == id);

            if (farm == null)
            {
                return NotFound();
            }

            return View(farm);
        }

        public async Task<IActionResult> Search(string farmName, string location, string koiType, int page = 1)
        {
            int pageSize = 8;

            // Tạo query cơ bản
            var query = _db.KoiFarms.AsQueryable();

            // Tìm kiếm theo tên farm
            if (!string.IsNullOrEmpty(farmName))
            {
                query = query.Where(f => f.FarmName.Contains(farmName));
            }

            // Tìm kiếm theo vị trí
            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(f => f.Location.Contains(location));
            }

            // Tìm kiếm theo loại koi
            if (!string.IsNullOrEmpty(koiType))
            {
                query = query.Where(f => f.SpecialVarieties
                    .Any(sv => sv.Variety.VarietyName.Contains(koiType) && sv.FarmId == f.FarmId));
            }

            // Gọi phương thức chung để lấy danh sách phân trang
            var (farmsOnPage, totalPages, totalFarms) = await GetPagedFarms(query, page, pageSize);

            // Truyền thông tin về trang hiện tại và tổng số trang vào ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View("Farm", farmsOnPage);
        }




    }
}
