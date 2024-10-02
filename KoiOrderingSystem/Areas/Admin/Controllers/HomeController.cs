using KoiOrderingSystem.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models; // Import your model namespace
using System.Linq; // For querying
using System.Threading.Tasks; // For async operations
using Microsoft.EntityFrameworkCore; // For database context if using EF Core

namespace KoiAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : BaseController
    {
        private readonly Koi88Context _db;

        // Constructor to inject the database context
        public HomeController(Koi88Context db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Updated OrderManagement action
        public async Task<IActionResult> OrderManagement()
        {
            // Fetch the list of bookings from the database
            var bookings = await _db.Bookings.ToListAsync();

            // Pass the list of bookings to the view
            return View(bookings);
        }
    }
}
