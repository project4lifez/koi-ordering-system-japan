using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Controllers
{
    public class RegisterController : Controller
    {
        private readonly Koi88Context _db;

        public RegisterController(Koi88Context db)
        {
            _db = db;
        }

        // GET: /Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Register
        [HttpPost]
        public async Task<IActionResult> Register(Account account)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = await _db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Set default values for RoleId and Status
                        account.RoleId = 1; // Default Role ID
                        account.Status = true; // Set Status to active

                        // Add the Account to the database
                        _db.Accounts.Add(account);
                        await _db.SaveChangesAsync();

                        // Create associated Customer record
                        var customer = new Models.Customer
                        {
                            AccountId = account.AccountId
                        };

                        // Save the customer record
                        _db.Customers.Add(customer);
                        await _db.SaveChangesAsync(); // Save the customer to generate the CustomerId

                        // Commit transaction
                        await transaction.CommitAsync();

                        // Redirect to the Login page after successful registration
                        return RedirectToAction("", "Login");
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction if there's an error
                        await transaction.RollbackAsync();
                        return BadRequest(new { error = ex.Message });
                    }
                }
            }

            // If validation fails, return the form with errors
            return View(account);
        }

        // Method to register a Google user
        public async Task<IActionResult> RegisterGoogleUser(string email)
        {
            var existingAccount = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email);

            if (existingAccount != null)
            {
                return RedirectToAction("", "Login");
            }

            var account = new Account
            {
                Email = email,
                RoleId = 1,
                Status = true
            };

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync();

            var customer = new Models.Customer { AccountId = account.AccountId };
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            return RedirectToAction("", "Login");
        }
    }
}
