﻿using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProfileDetailCusController : Controller
    {
        private readonly Koi88Context _db;

        public ProfileDetailCusController(Koi88Context db)
        {
            _db = db;
        }
        public IActionResult ProfileDetailCus(int accountId)
        {
            // Lấy thông tin tài khoản từ cơ sở dữ liệu, bao gồm thông tin khách hàng
            var account = _db.Accounts
                              .Include(a => a.Customers) // Nạp thông tin khách hàng
                              .Include(a => a.Role) // Nạp thông tin vai trò nếu cần
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
            bool isActive = status == "active";

            var account = _db.Accounts.FirstOrDefault(a => a.AccountId == accountId);
            if (account == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái
            account.Status = isActive;
            _db.SaveChanges();

            return RedirectToAction("ProfileDetailCus", "Admin", new { accountId = accountId });
        }
    }
}
