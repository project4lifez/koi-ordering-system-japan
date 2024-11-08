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
            // Check if the session variable is null and handle the default case
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != null && adminRoleId >= 2 && adminRoleId <= 5)
            {
                return RedirectToAction("Home", "Admin");
            }

            // Check for null and initialize lists if _db is not properly initialized
            var koiVarieties = _db?.Varieties
        ?.Where(v => new[] { 4, 5, 8, 6 }.Contains(v.VarietyId))
        ?.ToList() ?? new List<Variety>();

            var feedbacks = _db?.Feedbacks
                ?.Where(f => f.Rating == 5)
                ?.Include(f => f.Customer)
                    ?.ThenInclude(c => c.Account)
                ?.OrderByDescending(f => f.FeedbackId)
                ?.Take(3)
                ?.ToList() ?? new List<Feedback>();

            var koiFarms = _db?.KoiFarms
                ?.OrderByDescending(f => f.FarmId)
                ?.Take(3)
                ?.ToList() ?? new List<KoiFarm>();

            var koiFishes = _db?.KoiFishes
       ?.Where(k => k.VarietyId == 4)
       ?.OrderByDescending(k => k.KoiId)
       ?.Take(4)
       ?.ToList() ?? new List<KoiFish>();

            koiFishes.AddRange(_db?.KoiFishes
                ?.Where(k => k.VarietyId == 5)
                ?.OrderByDescending(k => k.KoiId)
                ?.Take(4)
                ?.ToList() ?? new List<KoiFish>());

            koiFishes.AddRange(_db?.KoiFishes
                ?.Where(k => k.VarietyId == 8)
                ?.OrderByDescending(k => k.KoiId)
                ?.Take(4)
                ?.ToList() ?? new List<KoiFish>());

            koiFishes.AddRange(_db?.KoiFishes
                ?.Where(k => k.VarietyId == 6)
                ?.OrderByDescending(k => k.KoiId)
                ?.Take(4)
                ?.ToList() ?? new List<KoiFish>());


            // Set ViewBag properties, ensuring they are not null
            ViewBag.KoiVarieties = koiVarieties;
            ViewBag.Feedbacks = feedbacks;
            ViewBag.KoiFarms = koiFarms;
            ViewBag.KoiFishes = koiFishes;
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

        public ActionResult Blog()
        {
            // Fetch blogs based on their Position and Status from the database
            var mainFeaturedBlog = _db.Blogs.FirstOrDefault(b => b.Position == 1 && b.Status == true);
            var sideBlogs = _db.Blogs.Where(b => b.Position >= 2 && b.Position <= 4 && b.Status == true).ToList();
            var trendingBlogs = _db.Blogs.Where(b => b.Position == 0 && b.Status == true).ToList();

            // Create the ViewModel and pass the data
            var model = new BlogViewModel
            {
                MainFeaturedBlog = mainFeaturedBlog,
                SideBlogs = sideBlogs,
                TrendingBlogs = trendingBlogs
            };

            return View(model);
        }

        // GET: /Home/BlogDetail/{id}
        public async Task<IActionResult> BlogDetail(int id)
        {
            // Fetch the blog using the provided id with Status = true
            var blog = await _db.Blogs.FirstOrDefaultAsync(b => b.BlogId == id && b.Status == true);
            if (blog == null)
            {
                return NotFound(); // Return 404 if the blog doesn't exist or is not active
            }

            // Create a BlogViewModel and populate it with the blog data
            var viewModel = new BlogViewModel
            {
                Blog = blog,
                SideBlogs = await _db.Blogs.Where(b => b.Position > 0 && b.Status == true).ToListAsync(),
                TrendingBlogs = await _db.Blogs.OrderByDescending(b => b.CreateAt).Where(b => b.Status == true).Take(5).ToListAsync(),
                MainFeaturedBlog = await _db.Blogs.FirstOrDefaultAsync(b => b.Position == 1 && b.Status == true)
            };

            return View(viewModel); // Pass the view model to the view
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
