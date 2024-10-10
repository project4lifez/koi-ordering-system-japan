using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Http; // For session management
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;


namespace KoiOrderingSystem.Controllers
{
    

    public class BookingController : Controller
    {

        private readonly Koi88Context _db;

        public BookingController(Koi88Context db)
        {
            _db = db;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if the user is logged in by checking the session
            if (HttpContext.Session.GetString("Username") == null)
            {
                // Redirect to login page if the user is not logged in
                context.Result = RedirectToAction("", "Login");
            }

            base.OnActionExecuting(context); // Call the base method
        }

        // GET: Booking/Create
        public IActionResult Create()
        {
            // Only logged-in users can access the booking form
            if (HttpContext.Session.GetString("Username") == null)
            {
                // Redirect to login page if the user is not logged in
                return RedirectToAction("", "Login");
            }

            return View();
        }

       

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Fullname,Phone,Email,Gender,Favoritefarm,FavoriteKoi,StartDate,EndDate,EstimatedCost,HotelAccommodation,Note")] Booking booking)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("", "Login");
            }

            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("", "Login");
            }

            if (ModelState.IsValid)
            {
                booking.CustomerId = customerId.Value;
                booking.BookingDate = DateOnly.FromDateTime(DateTime.Now);
                booking.Status = "Requested";
                booking.IsActive = true;

                _db.Add(booking);
                await _db.SaveChangesAsync();

                // Redirect to ViewBooking to show the list of bookings
                return RedirectToAction("","YourBooking");
            }

            return View(booking);
        }
        
        

        // GET: Booking/ViewBooking (Your Order page)
        public async Task<IActionResult> ViewBooking()
        {
            // Check if the user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("", "Login");
            }

            // Get CustomerId from session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("Home", "YourOrder");
            }

            // Get the list of bookings for the current user
            var bookings = await _db.Bookings
                                    .Where(b => b.CustomerId == customerId.Value)
                                    .ToListAsync();

            return View(bookings); // Pass the booking list to the view
        }
    }
}
