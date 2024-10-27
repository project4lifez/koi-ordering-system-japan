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

namespace KoiOrderingSystem.Controllers

{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public ActionResult Homepage()
        {
            // Lấy AdminRoleId từ session
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            // Nếu role từ 2 đến 5, điều hướng đến /Admin
            if (adminRoleId != null && adminRoleId >= 2 && adminRoleId <= 5)
            {
                return RedirectToAction("Home", "Admin");
            }

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

        public IActionResult Farm ()
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
