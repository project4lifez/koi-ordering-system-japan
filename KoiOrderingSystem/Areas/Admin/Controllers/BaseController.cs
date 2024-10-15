using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace KoiOrderingSystem.Controllers.Admin
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Get RoleId and other session variables
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");
            var adminLastName = HttpContext.Session.GetString("AdminLastname");
            var adminUsername = HttpContext.Session.GetString("AdminUsername");

            // Check if AdminRoleId exists, redirect to login if not
            if (adminRoleId == null)
            {
                filterContext.Result = new RedirectToActionResult("Login", "Login", null);
                return;
            }

            // Check if RoleId is within the allowed range (2 to 5)
            if (!IsAdminOrAllowedRole(adminRoleId.Value))
            {
                filterContext.Result = new RedirectToActionResult("AccessDenied", "Error", null);
                return;
            }

            // Set ViewBag for use in views
            ViewBag.Lastname = adminLastName;
            ViewBag.Username = adminUsername;
            ViewBag.AdminRoleId = adminRoleId;

            base.OnActionExecuting(filterContext);
        }

        // Check if RoleId is allowed
        private bool IsAdminOrAllowedRole(int roleId)
        {
            return roleId >= 2 && roleId <= 5; // Valid RoleId range
        }
    }
}
