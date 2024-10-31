using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Http; // For session management
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.InkML;

namespace KoiOrderingSystem.Controllers

{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly Koi88Context _db;
        public HomeController(ILogger<HomeController> logger, Koi88Context db)
        {
            _logger = logger;
            _db = db;
        }

        public ActionResult Homepage()
        {
       
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

           
            if (adminRoleId != null && adminRoleId >= 2 && adminRoleId <= 5)
            {
                return RedirectToAction("Home", "Admin");
            }

            // Fetch Koi varieties from the database
            var koiVarieties = _db.Varieties
                .OrderByDescending(v => v.VarietyId)
                 .Take(4)
                .ToList();

            var feedbacks = _db.Feedbacks
        .Where(f => f.Rating == 5) 
        .Include(f => f.Customer) 
            .ThenInclude(c => c.Account) 
        .OrderByDescending(f => f.FeedbackId) 
        .Take(3) 
        .ToList();

            var koiFarms = _db.KoiFarms
       .OrderByDescending(f => f.FarmId) // Assuming FarmId indicates recency
       .Take(3) // Take the top 3 farms
       .ToList();


            ViewBag.KoiVarieties = koiVarieties;
            ViewBag.Feedbacks = feedbacks;
            ViewBag.KoiFarms = koiFarms;
            ViewBag.Title = "KOI88 - Nishikigoi Ordering Service";

            return View();
        }

        public ActionResult BookingForm()
        {

            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("", "Login");
            }
            return View();
        }
        public ActionResult Create()
        {

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Farm()
        {
            return View();
        }

        public IActionResult Variety()
        {
            return View();
        }

        public IActionResult VarietyDetail()
        {
            return View();
        }
      

        public async Task<IActionResult> KoiVarieties()
        {
            var koiVarieties = await _db.Varieties
                .OrderByDescending(v => v.VarietyId) 
                .ToListAsync();

            return View(koiVarieties); 
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
