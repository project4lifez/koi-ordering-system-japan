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
			// Lấy thông tin tài khoản từ cơ sở dữ liệu, bao gồm thông tin vai trò
			var account = _db.Accounts
							 .Include(a => a.Role) // Nạp thông tin vai trò
							 .FirstOrDefault(a => a.AccountId == accountId);

			if (account == null)
			{
				return NotFound(); // Trả về lỗi nếu không tìm thấy tài khoản
			}

			return View(account); // Truyền model vào View
		}

		[HttpPost]
		public IActionResult UpdateStatus(int accountId, string status)
		{
			// Chuyển đổi string status thành bool
			bool isActive = status == "active";

			// Tìm tài khoản dựa trên AccountId
			var account = _db.Accounts.FirstOrDefault(a => a.AccountId == accountId);
			if (account == null)
			{
				return NotFound(); // Trả về lỗi nếu không tìm thấy tài khoản
			}

			// Cập nhật trạng thái
			account.Status = isActive;
			_db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

			// Trả về một phản hồi hoặc chuyển hướng về trang nào đó
			return RedirectToAction("ProfileDetail", "Admin", new { accountId = accountId });
		}

	}
}
