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
        public async Task<IActionResult> Farm()
        {
            var farms = await _db.KoiFarms
             .Include(f => f.SpecialVarieties)
                 .ThenInclude(sv => sv.Variety)
             .ToListAsync();
            foreach (var farm in farms)
            {
              
                if (farm.ImageUrl == null || farm.FarmName == null || farm.Location == null)
                {
                   
                }
            }
            return View(farms);
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

        public async Task<IActionResult> Search(string farmName, string location, string koiType)
        {
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


            var farms = await query
              .Include(f => f.SpecialVarieties)
                  .ThenInclude(sv => sv.Variety)
              .ToListAsync();

            var koiTypes = await _db.Varieties.Select(v => v.VarietyName).Distinct().ToListAsync();
            return View("Farm", farms);
        }

       
       

    }
}
