    using KoiOrderingSystem.Controllers.Admin;
    using KoiOrderingSystem.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace KoiOrderingSystem.Areas.Admin.Controllers
    {
	    [Area("Admin")]
	    public class UserListController : BaseController
	    {
		    private readonly Koi88Context _db;

		    public UserListController(Koi88Context db)
		    {
			    _db = db;
		    }
        public async Task<IActionResult> UserList(string? status, string searchQuery, int page = 1, int pageSize = 8)
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }

            var customerListQuery = _db.Accounts
                .Include(a => a.Customers)
                .Where(a => a.RoleId == 1); // Assuming RoleId 1 is for customers

            // Filter by status if provided
            if (!string.IsNullOrEmpty(status))
            {
                bool isActive = status == "active";
                customerListQuery = customerListQuery.Where(a => a.Status == isActive); // Assuming Status is bool
            }

            // Search by first name or last name if search query is provided
            if (!string.IsNullOrEmpty(searchQuery))
            {
                customerListQuery = customerListQuery.Where(a => a.Firstname.Contains(searchQuery) || a.Lastname.Contains(searchQuery));
            }

            // Get total number of users for pagination calculation
            int totalUsers = await customerListQuery.CountAsync();

            // Apply pagination
            var customerList = await customerListQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Set ViewBag properties for use in the view
            ViewBag.SelectedStatus = status;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            return View(customerList);
        }

    }
}
