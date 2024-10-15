using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KoiOrderingSystem.Views.Home
{
    public class HomeController : Controller
    {
        // The default action that returns the Homepage.cshtml view
        public IActionResult HomePage1()
        {
            return View("HomePage"); // Make sure the view is named "HomePage.cshtml"
        }
    }
}
