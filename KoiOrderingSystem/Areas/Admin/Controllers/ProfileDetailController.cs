using KoiOrderingSystem.Controllers.Admin;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProfileDetailController : BaseController
    {
        private readonly Koi88Context _db;

        public ProfileDetailController(Koi88Context db)
        {
            _db = db;
        }

		public IActionResult ProfileDetail(int accountId)
		{
			
			var account = _db.Accounts
							 .Include(a => a.Role) 
							 .FirstOrDefault(a => a.AccountId == accountId);

			if (account == null)
			{
				return NotFound(); 
			}

			return View(account); 
		}

		[HttpPost]
		public IActionResult UpdateStatus(int accountId, string status)
		{
			// Chuyển đổi string status thành bool
			bool isActive = status == "active";

		
			var account = _db.Accounts.FirstOrDefault(a => a.AccountId == accountId);
			if (account == null)
			{
				return NotFound(); 
			}

			
			account.Status = isActive;
			_db.SaveChanges(); 

			
			return RedirectToAction("ProfileDetail", "Admin", new { accountId = accountId });
		}

	}
}
