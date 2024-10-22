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
        public async Task<IActionResult> Variety()
        {
            var varieties = await _db.Varieties.ToListAsync();
            foreach (var variety in varieties)
            {

                if (variety.ImageUrl == null || variety.VarietyName == null || variety.Description == null)
                {

                }
            }
            return View(varieties);
        }

        public async Task<IActionResult> VarietyDetail(int id)
        {
            
            var variety = await _db.Varieties
                .Include(v => v.KoiFishes) 
                .FirstOrDefaultAsync(v => v.VarietyId == id);

            if (variety == null)
            {
                return NotFound(); 
            }

            return View(variety); 
        }

        public async Task<IActionResult> Search(string title, string variety, string breeder)
        {
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

            var varieties = await query
                .Include(v => v.SpecialVarieties)
                    .ThenInclude(sv => sv.Farm)
                .ToListAsync();

            // Lấy danh sách tất cả các FarmName
            var breeders = await _db.KoiFarms.Select(f => f.FarmName).Distinct().ToListAsync();
            var allVarieties = await _db.Varieties.Select(v => v.VarietyName).Distinct().ToListAsync();
            // Trả về view với cả varieties và breeders
            return View("Variety", varieties);
        }
    }
}
