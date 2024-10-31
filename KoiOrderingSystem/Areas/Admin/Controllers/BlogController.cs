using KoiOrderingSystem.Controllers.Admin;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : BaseController
    {
        private readonly Koi88Context _db;

        public BlogController(Koi88Context db)
        {
            _db = db;
        }

        // GET: /Admin/Blog/BlogList
        public async Task<IActionResult> BlogList(string query, int page = 1)
        {
            ViewBag.CurrentPage = page;
            int pageSize = 8; // Số lượng blog trên mỗi trang
            var blogs = _db.Blogs.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                blogs = blogs.Where(b => b.Heading.Contains(query) || b.Link.Contains(query));
            }

            ViewBag.TotalPages = (int)System.Math.Ceiling(await blogs.CountAsync() / (double)pageSize);
            var blogList = await blogs.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return View(blogList);
        }

        // GET: /Admin/Blog/CreateBlog
        public IActionResult CreateBlog()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlog(Blog model, IFormFile Image, string Status)
        {
            // Convert the string Status value to boolean
            model.Status = Status == "1";

            // Create a new Blog instance
            var newBlog = new Blog
            {
                Heading = model.Heading,
                Link = model.Link,
                Status = model.Status, // Set with the converted boolean value
                Position = model.Position,
                CreateAt = DateTime.UtcNow // Set the creation date to now
            };

            // Check if the selected position is already taken
            // Allow multiple entries for Position = 0
            if (newBlog.Position != 0)
            {
                var existingBlog = _db.Blogs.FirstOrDefault(b => b.Position == newBlog.Position);
                if (existingBlog != null)
                {
                    // Handle confirmation logic for position conflict
                    ViewBag.ExistingBlog = existingBlog;
                    return View("ConfirmPositionChange", newBlog); // Redirect to confirmation view
                }
            }

            // Process image upload
            if (Image != null && Image.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(Image.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(Image.FileName);
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Blog");
                var filePath = Path.Combine(directoryPath, fileName);

                // Ensure the directory exists
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                newBlog.Image = "/images/Blog/" + fileName; // Set image URL
            }

            // Add new blog to the database
            _db.Blogs.Add(newBlog);
            await _db.SaveChangesAsync(); // This will generate BlogId

            return Redirect("/Admin/Blog/BlogList");
        }










        private async Task<bool> ConfirmPositionChange(int blogId)
        {
            // Logic xác nhận sẽ ở đây
            // Thực hiện hiển thị modal xác nhận cho người dùng và trả về true hoặc false
            // Ví dụ: dùng SignalR để gửi thông báo tới người dùng, hoặc hiển thị alert trên client-side.
            return true; // Đây là một giá trị giả định, hãy thay thế bằng logic thực tế của bạn.
        }

        public async Task<IActionResult> UpdateBlog(int id)
        {
            var blog = await _db.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // GET: /Admin/Blog/UpdateBlog/{id}
        [HttpPost]
        public async Task<IActionResult> UpdateBlog(Blog model, IFormFile Image, string Status)
        {
            var existingBlog = await _db.Blogs.FindAsync(model.BlogId);
            if (existingBlog == null)
            {
                return NotFound(); // Handle not found scenario
            }

            // Convert the string Status value to boolean
            existingBlog.Status = Status == "1"; // Convert string to boolean: "1" = true, "0" = false

            // Update the properties
            existingBlog.Heading = model.Heading;
            existingBlog.Link = model.Link;
            existingBlog.Position = model.Position;

            // Optional: Update the CreateAt property (if you want to change it on every update)
            existingBlog.CreateAt = DateTime.UtcNow; // Update CreateAt to current date/time on update

            // Process image upload if provided
            if (Image != null && Image.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(Image.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(Image.FileName);
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Blog");
                var filePath = Path.Combine(directoryPath, fileName);

                // Ensure the directory exists
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                existingBlog.Image = "/images/Blog/" + fileName; // Set new image URL
            }

            // Save changes
            await _db.SaveChangesAsync(); // This will update the existing blog

            return Redirect("/Admin/Blog/BlogList");
        }



        // POST: /Admin/Blog/DeleteBlog
        [HttpPost]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _db.Blogs.FindAsync(id);
            if (blog != null)
            {
                _db.Blogs.Remove(blog);
                await _db.SaveChangesAsync();
            }

            return Redirect("/Admin/Blog/BlogList");
        }
    }
}
