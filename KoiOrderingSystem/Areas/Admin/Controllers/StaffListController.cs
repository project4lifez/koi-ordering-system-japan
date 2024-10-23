using KoiOrderingSystem.Controllers.Admin;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StaffListController : BaseController
    {
        private readonly Koi88Context _db;

        public StaffListController(Koi88Context db)
        {
            _db = db;
        }
        public async Task<IActionResult> StaffList(int? roleId)
        {
            // Retrieve RoleID from session
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            // Check if the user has the manager role (RoleID = 2)
            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }

            // Fetch staff information with RoleId of 2, 3, 4, or 5, and apply filtering based on roleId if provided
            var staffQuery = _db.Accounts
                .Include(a => a.Role) // Include related Role information
                .Where(a => a.RoleId != null && (a.RoleId == 2 || a.RoleId == 3 || a.RoleId == 4 || a.RoleId == 5)); // Filter only RoleId 2, 3, 4, 5

            // If a specific roleId is selected, apply additional filtering
            if (roleId != null)
            {
                staffQuery = staffQuery.Where(a => a.RoleId == roleId);
            }

            var staffList = await staffQuery.ToListAsync();
            ViewBag.SelectedRoleId = roleId;
            return View(staffList); 
        }
    }
}
