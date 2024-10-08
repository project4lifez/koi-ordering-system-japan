using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Localization;
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
            // Lấy ngôn ngữ từ Cookie, nếu không có thì mặc định là tiếng Anh ("en")
            var culture = Request.Cookies[".AspNetCore.Culture"] ?? "en";
            var model = new HomeViewModel();

            // Gán các giá trị cho ViewModel dựa trên ngôn ngữ
            switch (culture)
            {
                case var c when c.Contains("ja"):
                    model.LanguageSelected = "ja";
                    model.Home = KoiOrderingSystem.Localization.LocalizationJA.Home;
                    model.Services = KoiOrderingSystem.Localization.LocalizationJA.Services;
                    model.About = KoiOrderingSystem.Localization.LocalizationJA.About;
                    model.Blog = KoiOrderingSystem.Localization.LocalizationJA.Blog;
                    break;

                default:
                    model.LanguageSelected = "en";
                    model.Home = KoiOrderingSystem.Localization.LocalizationEN.Home;
                    model.Services = KoiOrderingSystem.Localization.LocalizationEN.Services;
                    model.About = KoiOrderingSystem.Localization.LocalizationEN.About;
                    model.Blog = KoiOrderingSystem.Localization.LocalizationEN.Blog;
                    break;
            }

            model.Title = "KOI88 - Nishikigoi Ordering Service";
            return View(model); // Trả về View với model đã xử lý
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture)
        {
            // Lưu ngôn ngữ đã chọn vào Cookie
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            // Chuyển hướng về trang trước khi người dùng đổi ngôn ngữ
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }

    public class HomeViewModel
    {
        public string Title { get; set; }
        public string LanguageSelected { get; set; }
        public string Home { get; set; }
        public string Services { get; set; }
        public string About { get; set; }
        public string Blog { get; set; }
    }
}
