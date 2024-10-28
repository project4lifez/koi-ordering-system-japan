using KoiOrderingSystem.Controllers.Admin;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class RoleListController : BaseController
    {
        private readonly Koi88Context _db;

        public RoleListController(Koi88Context db)
        {
            _db = db;
        }
        public IActionResult RoleList()
        {
            
            var roles = _db.Roles.ToList();
            return View(roles); 
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateRole(string RoleName)
        {
            if (string.IsNullOrEmpty(RoleName))
            {
                ModelState.AddModelError("", "Role name is required.");
                return View();
            }

            // Create a new role instance and save it to the database
            var newRole = new Role
            {
                Name = RoleName
            };

            _db.Roles.Add(newRole);
            _db.SaveChanges();

            return RedirectToAction("RoleList","Admin");
        }

        [HttpPost]
        public IActionResult DeleteRole(int id)
        {
            // Find the role by ID
            var role = _db.Roles.Find(id);
            if (role == null)
            {
                return NotFound();
            }

            _db.Roles.Remove(role);
            _db.SaveChanges();

            return RedirectToAction("RoleList", "Admin");
        }

        [HttpGet]
        public IActionResult UpdateRole(int id)
        {
            var role = _db.Roles.Find(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: /Admin/RoleList/UpdateRole
        [HttpPost]
        public IActionResult UpdateRole(Role updatedRole)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedRole);
            }

            var existingRole = _db.Roles.Find(updatedRole.RoleId);
            if (existingRole == null)
            {
                return NotFound();
            }

            existingRole.Name = updatedRole.Name;
            _db.SaveChanges();
            return Redirect($"/Admin/RoleList/UpdateRole/{updatedRole.RoleId}");
        }
    }
}
