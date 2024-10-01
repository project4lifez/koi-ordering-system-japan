using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters; // Required for ActionExecutingContext
using Microsoft.AspNetCore.Http;

namespace KoiOrderingSystem.Controllers.Admin
{
    public class BaseController : Controller
    {
        // Đảm bảo rằng phương thức này có mức truy cập public như lớp cơ sở Controller
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Lấy RoleId từ Session
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var lastName = HttpContext.Session.GetString("Lastname");

            // Kiểm tra nếu RoleId không tồn tại, chuyển hướng về trang đăng nhập
            if (roleId == null)
            {
                filterContext.Result = new RedirectToActionResult("Login", "Login", null);
                return;
            }

            // Gom nhóm các RoleId từ 2 đến 5 thành 1 nhóm (admin và các quyền liên quan)
            if (!IsAdminOrAllowedRole(roleId.Value))
            {
                // Nếu không phải là nhóm RoleId từ 2 đến 5, chuyển hướng về trang Access Denied
                filterContext.Result = new RedirectToActionResult("AccessDenied", "Error", null);
            }

            // Có thể sử dụng Lastname ở đây nếu cần
            ViewBag.Lastname = lastName;

            base.OnActionExecuting(filterContext);
        }

        // Hàm kiểm tra xem RoleId có thuộc nhóm admin hoặc các role có quyền truy cập không
        private bool IsAdminOrAllowedRole(int roleId)
        {
            // Nhóm RoleId hợp lệ là từ 2 đến 5 (admin và các quyền liên quan)
            return roleId >= 2 && roleId <= 5;
        }
    }
}
