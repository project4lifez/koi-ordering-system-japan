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
		public async Task<IActionResult> StaffList(int? roleId, string searchQuery, int page = 1, int pageSize = 4)
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
				.Where(a => a.RoleId != null && (a.RoleId == 2 || a.RoleId == 3 || a.RoleId == 4 || a.RoleId == 5));

			// Apply role filter if provided
			if (roleId != null)
			{
				staffQuery = staffQuery.Where(a => a.RoleId == roleId);
			}

			// Apply search query filter if provided
			if (!string.IsNullOrEmpty(searchQuery))
			{
				staffQuery = staffQuery.Where(a => a.Firstname.Contains(searchQuery) || a.Lastname.Contains(searchQuery));
			}

			// Pagination: Calculate total number of items and apply pagination
			int totalItems = await staffQuery.CountAsync();
			var staffList = await staffQuery
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			// Prepare ViewBag for selected filters and pagination data
			ViewBag.SelectedRoleId = roleId;
			ViewBag.SearchQuery = searchQuery;
			ViewBag.CurrentPage = page;
			ViewBag.PageSize = pageSize;
			ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

			return View(staffList);
		}
	}
}
