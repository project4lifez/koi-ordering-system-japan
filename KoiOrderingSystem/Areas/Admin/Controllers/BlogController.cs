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
        public async Task<IActionResult> BlogList(string query, int? position, int page = 1)
        {
            ViewBag.CurrentPage = page;
            int pageSize = 8; // Number of blogs per page
            var blogs = _db.Blogs.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                blogs = blogs.Where(b => b.Heading.Contains(query) || b.Link.Contains(query));
            }

            if (position.HasValue)
            {
                blogs = blogs.Where(b => b.Position == position.Value); // Assuming `Position` is a property in the Blog model
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
            model.Status = Status == "1";
            var newBlog = new Blog
            {
                Heading = model.Heading,
                Link = model.Link,
                Status = model.Status,
                Position = model.Position,
                CreateAt = DateTime.UtcNow
            };

            // Check if selected position is already taken (positions 1-4 must be unique)
            // Check if selected position is already taken (positions 1-4 must be unique)
            if (newBlog.Position >= 1 && newBlog.Position <= 4)
            {
                var existingBlog = await _db.Blogs.FirstOrDefaultAsync(b => b.Position == newBlog.Position);
                if (existingBlog != null)
                {
                    ViewBag.ConflictMessage = $"Position {newBlog.Position} is already occupied by Blog ID {existingBlog.BlogId}. Do you want to override this position?";

                    // Store only the necessary fields in TempData
                    TempData["ExistingBlogId"] = existingBlog.BlogId;
                    TempData["NewBlog_Heading"] = newBlog.Heading;
                    TempData["NewBlog_Link"] = newBlog.Link;
                    TempData["NewBlog_Status"] = newBlog.Status;
                    TempData["NewBlog_Position"] = newBlog.Position;

                    return View("CreateBlog"); // Return to CreateBlog view with the modal prompt
                }
            }

            // Process image upload
            if (Image != null && Image.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(Image.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(Image.FileName);
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Blog");
                var filePath = Path.Combine(directoryPath, fileName);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                newBlog.Image = "/images/Blog/" + fileName;
            }

            // Add new blog to the database if no conflict or after confirmation
            _db.Blogs.Add(newBlog);
            await _db.SaveChangesAsync();

            return Redirect("/Admin/Blog/BlogList");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOverride(int existingBlogId, Blog newBlog, IFormFile Image, string Status)
        {
            // Set the status of the new blog
            newBlog.Status = Status == "1";

            // Update the existing blog to Position = 0 if it has the same position
            var conflictingBlog = await _db.Blogs.FirstOrDefaultAsync(b => b.Position == newBlog.Position && b.BlogId != newBlog.BlogId);
            if (conflictingBlog != null)
            {
                conflictingBlog.Position = 0;
                _db.Blogs.Update(conflictingBlog);
            }

            // Process image upload if provided
            if (Image != null && Image.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(Image.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(Image.FileName);
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Blog");
                var filePath = Path.Combine(directoryPath, fileName);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                newBlog.Image = "/images/Blog/" + fileName;
            }

            // Save the new blog with the desired position and updated status
            _db.Blogs.Add(newBlog);
            await _db.SaveChangesAsync();

            return Redirect("/Admin/Blog/BlogList");
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
