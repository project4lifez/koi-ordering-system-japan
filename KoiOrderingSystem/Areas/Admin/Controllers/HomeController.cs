using KoiOrderingSystem.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;
using KoiOrderingSystem.Models; // Import your model namespace
using System.Linq; // For querying
using System.Threading.Tasks; // For async operations
using Microsoft.EntityFrameworkCore; // For database context if using EF Core
using System;
using PagedList;
using OfficeOpenXml; // For EPPlus
using System.IO; // For MemoryStream
using Microsoft.AspNetCore.Http; // For FileContentResult
using System.Globalization; // For culture-specific formatting
using ClosedXML.Excel;



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
            // Sử dụng năm hiện tại nếu không có năm nào được chọn
            var year = selectedYear ?? DateTime.Now.Year;

            var validStatuses = new[] { "Confirmed", "Checked in", "Checked out", "Delivering", "Delivered" };

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
                .Where(b => b.BookingDate.HasValue && b.BookingDate.Value.Year == year) // Chỉ lấy dữ liệu theo năm đã chọn
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

            // New - Fetch Booking Status Counts for Pie Chart, filtered by selected year
            var bookingStatusCounts = await _db.Bookings
                .Where(b => b.BookingDate.HasValue && b.BookingDate.Value.Year == year) // Lọc theo năm đã chọn
                .GroupBy(b => b.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            // Prepare data for Pie Chart, ensuring it only includes statuses for "Requested", "Confirmed", and "Canceled"
            var pieChartData = new Dictionary<string, int>
{
    { "Requested", bookingStatusCounts
        .Where(x => new[] { "Requested", "Rejected", "Processing", "Accepted" }
        .Contains(x.Status))
        .Sum(x => x.Count) },
    { "Confirmed", bookingStatusCounts
        .Where(x => new[] { "Confirmed", "Checked in", "Checked out", "Delivering", "Delivered" }
        .Contains(x.Status))
        .Sum(x => x.Count) },
    { "Canceled", bookingStatusCounts.FirstOrDefault(x => x.Status == "Canceled")?.Count ?? 0 }
};

            // Pass data to the view
            ViewBag.SelectedYear = year;
            ViewBag.TotalMonthlyRevenue = monthlyRevenueArray.Sum();
            ViewBag.MonthlyRevenue = monthlyRevenueArray; // Array for chart
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalTripsSuccess = totalTripsSuccess;
            ViewBag.RecentOrders = recentOrders; // Pass recent orders to the view

            // Pass Pie Chart data to the view
            ViewBag.PieChartData = pieChartData;

            return View();
        }

        // POST: Export the Excel report for the selected year
        [HttpPost]
        public IActionResult ExportToExcel(int selectedYear)
        {
            var validStatuses = new[] { "Confirmed", "Checked in", "Checked out", "Delivering", "Delivered" };

            // Query data for the selected year
            var exportData = _db.Bookings
                .Where(b => b.BookingDate.HasValue && b.BookingDate.Value.Year == selectedYear && validStatuses.Contains(b.Status))
                .Select(b => new
                {
                    Month = b.BookingDate.Value.Month,
                    BookingId = b.BookingId,
                    Customer = b.Fullname,
                    TripName = b.Trip.TripName, // Assuming Booking has Trip and Trip has TripName
                    Price = b.QuotedAmount,
                    BookingDate = b.BookingDate,
                    Status = b.Status
                })
                .ToList();

            // Monthly revenue
            var monthlyRevenue = exportData
                .GroupBy(b => b.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalRevenue = g.Sum(b => b.Price)
                })
                .OrderBy(r => r.Month)
                .ToList();

            // Total count of "Delivered" and "Canceled" bookings
            var deliveredCount = _db.Bookings
                .Count(b => b.BookingDate.HasValue && b.BookingDate.Value.Year == selectedYear && b.Status == "Delivered");

            var canceledCount = _db.Bookings
                .Count(b => b.BookingDate.HasValue && b.BookingDate.Value.Year == selectedYear && b.Status == "Canceled");

            // Generate Excel file using ClosedXML
            using (var workbook = new XLWorkbook())
            {
                // Worksheet 1: Detailed Report
                var worksheet = workbook.Worksheets.Add("RevenueReport");

                // Add header row
                worksheet.Cell(1, 1).Value = "Booking Date";
                worksheet.Cell(1, 2).Value = "Booking ID";
                worksheet.Cell(1, 3).Value = "Customer";
                worksheet.Cell(1, 4).Value = "Trip Name";
                worksheet.Cell(1, 5).Value = "Price (USD)";

                // Add data rows for bookings
                for (int i = 0; i < exportData.Count; i++)
                {
                    var row = i + 2;
                    worksheet.Cell(row, 1).Value = exportData[i].BookingDate?.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 2).Value = exportData[i].BookingId;
                    worksheet.Cell(row, 3).Value = exportData[i].Customer;
                    worksheet.Cell(row, 4).Value = exportData[i].TripName;
                    worksheet.Cell(row, 5).Value = exportData[i].Price;
                }

                // Worksheet 2: Summary Report
                var summaryWorksheet = workbook.Worksheets.Add("Summary");

                // Add header for monthly revenue
                summaryWorksheet.Cell(1, 1).Value = "Month";
                summaryWorksheet.Cell(1, 2).Value = "Total Revenue (USD)";

                // Add monthly revenue rows
                for (int i = 0; i < monthlyRevenue.Count; i++)
                {
                    var row = i + 2;
                    summaryWorksheet.Cell(row, 1).Value = monthlyRevenue[i].Month;
                    summaryWorksheet.Cell(row, 2).Value = monthlyRevenue[i].TotalRevenue;
                }

                // Add delivered and canceled counts
                var deliveredRow = monthlyRevenue.Count + 3;
                summaryWorksheet.Cell(deliveredRow, 1).Value = "Total Delivered Bookings";
                summaryWorksheet.Cell(deliveredRow, 2).Value = deliveredCount;

                var canceledRow = deliveredRow + 1;
                summaryWorksheet.Cell(canceledRow, 1).Value = "Total Canceled Bookings";
                summaryWorksheet.Cell(canceledRow, 2).Value = canceledCount;

                // Convert workbook to a memory stream and return as a file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"RevenueReport_{selectedYear}.xlsx");
                }
            }
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

        public async Task<IActionResult> OrderList(string searchQuery, string statusFilter, int page = 1, int pageSize = 15)
        {
            // Tạo truy vấn ban đầu
            var bookingsQuery = _db.Bookings.Include(b => b.Trip).AsQueryable();

            // Thực hiện tìm kiếm
            if (!string.IsNullOrEmpty(searchQuery))
            {
                bookingsQuery = bookingsQuery.Where(b =>
                    (b.BookingId.ToString().Contains(searchQuery)) ||
                    (b.Fullname != null && b.Fullname.ToLower().Contains(searchQuery.ToLower())) ||
                    (b.Trip != null && b.Trip.TripName != null && b.Trip.TripName.ToLower().Contains(searchQuery.ToLower())) ||
                    (b.QuotedAmount != null && b.QuotedAmount.ToString().Contains(searchQuery))
                );
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(statusFilter))
            {
                bookingsQuery = bookingsQuery.Where(b => b.Status.ToLower() == statusFilter.ToLower());
            }

            // Tổng số lượng đơn hàng sau khi tìm kiếm và lọc
            var totalBookings = await bookingsQuery.CountAsync();

            // Lấy danh sách đơn hàng với phân trang
            var bookings = await bookingsQuery
                .OrderBy(b => b.BookingId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Truyền dữ liệu xuống View
            ViewBag.TotalBookings = totalBookings;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalBookings / pageSize);
            ViewBag.SearchQuery = searchQuery;
            ViewBag.StatusFilter = statusFilter;

            return View(bookings);
        }



    }
}
