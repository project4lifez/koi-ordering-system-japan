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
        public async Task<IActionResult> UserList(string? status)
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }

            var customerListQuery = _db.Accounts
                .Include(a => a.Customers)
                .Where(a => a.RoleId == 1); 

           
            if (!string.IsNullOrEmpty(status))
            {
                bool isActive = status == "active"; 

                customerListQuery = customerListQuery.Where(a => a.Status == isActive); // Giả sử Status là bool
            }

            var customerList = await customerListQuery.ToListAsync();
            ViewBag.SelectedStatus = status; // Lưu trạng thái đã chọn vào ViewBag
            return View(customerList);
        }
    }
    }
