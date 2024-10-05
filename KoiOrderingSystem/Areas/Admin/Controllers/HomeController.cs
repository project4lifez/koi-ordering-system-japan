using Microsoft.AspNetCore.Mvc;

namespace KoiAdmin.Areas.Admin.Controllers
{
	[Area("admin")]
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult OrderManagement()
		{
			return View();
		}
	}
}
