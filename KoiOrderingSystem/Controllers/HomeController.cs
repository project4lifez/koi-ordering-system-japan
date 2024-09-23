using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            ViewBag.Title = "KOI88 - Nishikigoi Ordering Service";
            return View();
        }



        public IActionResult Privacy()
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
