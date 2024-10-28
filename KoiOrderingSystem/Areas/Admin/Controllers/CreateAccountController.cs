using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CreateAccountController : Controller
    {
        private readonly Koi88Context _db;

        public CreateAccountController(Koi88Context db)
        {
            _db = db;
        }
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(Account model, IFormFile avatar)
        {
            // Kiểm tra nếu người dùng có RoleID = 2
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId"); // Giả sử bạn lưu RoleID trong session
            if (adminRoleId != 2)
            {
                return Unauthorized(); // Trả về phản hồi không được phép hoặc chuyển hướng nếu cần
            }

            if (ModelState.IsValid)
            {
                // Tải lên avatar và lưu đường dẫn
                if (avatar != null && avatar.Length > 0)
                {
                    // Đặt thư mục tải lên là wwwroot/images/avatars
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars");
                    var filePath = Path.Combine(uploads, avatar.FileName);

                    // Đảm bảo thư mục tồn tại
                    Directory.CreateDirectory(uploads);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatar.CopyToAsync(stream);
                    }

                    // Cập nhật ImageUrl trong model
                    model.ImageUrl = $"/images/avatars/{avatar.FileName}"; // Đường dẫn hình ảnh được cập nhật
                }
				model.Status = true;
				// Lưu tài khoản người dùng
				_db.Accounts.Add(model);
                await _db.SaveChangesAsync();

                if (model.RoleId == 1)
                {
                    var customer = new Customer
                    {
                        AccountId = model.AccountId // Gán AccountId từ model
                    };

                    _db.Customers.Add(customer);
                    await _db.SaveChangesAsync(); // Lưu Customer
                }

                return Redirect("/Admin/CreateAccount/CreateAccount");
            }

            return View(model); // Trả về view với các lỗi xác thực
        }


    }
}
