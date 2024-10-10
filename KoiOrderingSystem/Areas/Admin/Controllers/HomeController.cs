using KoiOrderingSystem.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models; // Import your model namespace
using System.Linq; // For querying
using System.Threading.Tasks; // For async operations
using Microsoft.EntityFrameworkCore; // For database context if using EF Core
using System;


namespace KoiAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : BaseController
    {
        private readonly Koi88Context _db;

        // Constructor to inject the database context
        public HomeController(Koi88Context db)
        {
            _db = db;
        }

        // Updated Home action to pass the statistics to the view
        public async Task<IActionResult> Home(int? selectedYear)
        {
            // Use the current year by default if no year is selected
            var year = selectedYear ?? DateTime.Now.Year;

            var validStatuses = new[] { "Payment Completed", "Checked in", "Checked out", "Delivering", "Delivered" };

            // Total Orders for the selected year
            var totalOrders = await _db.Bookings
                .Where(b => b.BookingDate.HasValue && b.BookingDate.Value.Year == year)
                .CountAsync();

            // Monthly Revenue for each month of the selected year
            var monthlyRevenues = await _db.Bookings
                .Where(b => b.BookingDate.HasValue && b.BookingDate.Value.Year == year && validStatuses.Contains(b.Status))
                .GroupBy(b => b.BookingDate.Value.Month)
                .Select(g => new { Month = g.Key, TotalRevenue = g.Sum(b => b.QuotedAmount ?? 0) })
                .ToListAsync();

            // Create an array of 12 months with default value 0 for revenue
            var monthlyRevenueArray = new decimal[12];
            foreach (var revenue in monthlyRevenues)
            {
                monthlyRevenueArray[revenue.Month - 1] = revenue.TotalRevenue;
            }

            // Total Revenue for the selected year
            var totalRevenue = await _db.Bookings
                .Where(b => b.BookingDate.HasValue && b.BookingDate.Value.Year == year && validStatuses.Contains(b.Status))
                .SumAsync(b => b.QuotedAmount ?? 0);

            // Total Trips Success for the selected year
            var totalTripsSuccess = await _db.Bookings
                .CountAsync(b => b.Status == "Delivered" && b.BookingDate.Value.Year == year);

            // Fetch recent orders, limit to 5 most recent, using Booking.FullName and BookingId
            var recentOrders = await _db.Bookings
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .Select(b => new
                {
                    BookingId = b.BookingId, // Add BookingId here
                    FullName = b.Fullname,   // Assuming Booking has FullName property
                    BookingDate = b.BookingDate,
                    Status = b.Status
                })
                .ToListAsync();

            // Pass data to the view
            ViewBag.SelectedYear = year;
            ViewBag.TotalMonthlyRevenue = monthlyRevenueArray.Sum();
            ViewBag.MonthlyRevenue = monthlyRevenueArray; // Array for chart
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalTripsSuccess = totalTripsSuccess;
            ViewBag.RecentOrders = recentOrders; // Pass recent orders to the view

            return View();
        }







        // Updated OrderManagement action
        public async Task<IActionResult> OrderManagement()
        {
            // Fetch the list of bookings from the database, including the related Trip entity
            var bookings = await _db.Bookings
                .Include(b => b.Trip) // Include the related Trip entity so you can access TripName
                .ToListAsync();

            // Pass the list of bookings to the view
            return View(bookings);
        }
    
        public async Task<IActionResult> OrderList()
        {
            // Fetch the list of bookings from the database, including the related Trip entity
            var bookings = await _db.Bookings
                .Include(b => b.Trip) // Include the related Trip entity so you can access TripName
                .ToListAsync();


            // Pass the list of bookings to the view
            return View(bookings);
        }
    }
}
