using KoiOrderingSystem.Controllers.Admin;
using KoiOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace KoiOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManagerController : BaseController
    {
        private readonly Koi88Context _db;
        private readonly SmtpSettings _smtpSettings;

        public ManagerController(Koi88Context db, IOptions<SmtpSettings> smtpSettings)
        {
            _db = db;
            _smtpSettings = smtpSettings.Value;
        }

        // Action to display the Manager page
        public IActionResult Manager(int id)
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId == null || adminRoleId != 2)
            {
                return NotFound("You do not have permission to access this page.");
            }

            var booking = _db.Bookings
                .Include(b => b.Trip)
                .ThenInclude(t => t.TripDetails)
                .Include(b => b.Po)
                .ThenInclude(po => po.Podetails)
                .ThenInclude(podetail => podetail.Koi)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }

            return View(booking);
        }

        // Action to update status
        [HttpPost]
        public async Task<IActionResult> UpdateStatusManager(int bookingId, string status)
        {
            var booking = _db.Bookings
                             .Include(b => b.BookingPayments)
                             .Include(b => b.Po).ThenInclude(po => po.Podetails)
                             .Include(b => b.Trip)
                             .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking != null)
            {
                if (booking.Status == "Requested")
                {
                    ModelState.AddModelError("", "Cannot update status from 'Requested'.");
                    return View("Manager", booking);
                }

                booking.Status = status;
                booking.QuoteSentDate = DateOnly.FromDateTime(DateTime.Now);

                if (status == "Accepted")
                {
                    booking.QuoteApprovedDate = DateOnly.FromDateTime(DateTime.Now);

                    if (!booking.BookingPayments.Any())
                    {
                        var bookingPayment = new BookingPayment
                        {
                            Status = "Pending",
                            BookingId = booking.BookingId
                        };

                        _db.BookingPayments.Add(bookingPayment);
                    }
                }

                if (status == "Rejected" || status == "Canceled" || status == "Lost Deposit" || status == "Refunding" || status == "Refunded")
                {
                    booking.QuoteApprovedDate = null;
                }

                if (status == "Lost Deposit")
                {
                    TempData["InfoMessage"] = "Booking status updated to 'Lost Deposit'.";
                    await SendLostDepositEmail(booking); // Gửi email khi trạng thái là "Lost Deposit"                }
                }
                else if (status == "Refunding")
                {
                    TempData["InfoMessage"] = "Booking status updated to 'Refunding'. Processing refund.";


                    // Calculate Deposit Amount and Compensation
                    decimal depositAmount = booking.Po?.Podetails?
                                            .Where(pd => pd.PoId == booking.Po.PoId)
                                            .GroupBy(pd => pd.FarmId)
                                            .Sum(group => group.FirstOrDefault()?.Deposit ?? 0) ?? 0;

                    // Điều chỉnh lại công thức để cộng depositAmount vào tổng rồi mới nhân 0.25
                    decimal compensationAmount = (depositAmount + (booking.Po?.TotalAmount ?? 0)) * 0.25m; 
                    decimal totalRefundAmount = depositAmount + compensationAmount;

                    await SendRefundNotificationEmail(booking, depositAmount, compensationAmount, totalRefundAmount);
                }
                else if (status == "Refunded")
                {
                    TempData["InfoMessage"] = "Booking status updated to 'Refunded'. Refund completed.";

                }

                _db.SaveChanges();
                TempData["SuccessMessage"] = $"Status updated to '{status}' successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Booking not found.";
            }

            return Redirect("Manager?id=" + bookingId);
        }

        // Method to send refund notification email
        private async Task SendRefundNotificationEmail(Booking booking, decimal depositAmount, decimal compensationAmount, decimal totalRefundAmount)
        {
            var email = booking.Email;
            var tripName = booking.Trip?.TripName ?? "Unknown Trip";
            var fullname = booking.Fullname;
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SmtpUser),
                Subject = "Refund Notification for Your Koi88 Order",
                IsBodyHtml = true,
                Body = $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f7f9fc !important;
                    color: #333 !important;
                    padding: 20px !important;
                }}
                .container {{
                    max-width: 600px !important;
                    margin: auto !important;
                    background: #ffffff !important;
                    padding: 30px !important;
                    border-radius: 8px !important;
                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1) !important;
                    border: 1px solid #e0e0e0 !important;
                    overflow: visible !important;
                }}
                h1 {{
                    color: #007BFF !important;
                    font-size: 24px !important;
                    margin-bottom: 20px !important;
                    text-align: center !important;
                }}
                p {{
                    font-size: 16px !important;
                    line-height: 1.6 !important;
                    color: #555 !important;
                    margin-bottom: 15px !important;
                }}
                .highlight {{
                    color: #007BFF !important;
                    font-weight: bold !important;
                }}
                .table-container {{
                    margin: 20px 0 !important;
                    font-size: 16px !important;
                }}
                table {{
                    width: 100% !important;
                    border-collapse: collapse !important;
                    border: 1px solid #e0e0e0 !important;
                }}
                th {{
                    background-color: #007BFF !important;
                    color: white !important;
                    padding: 10px !important;
                    text-align: left !important;
                }}
                td {{
                    padding: 10px !important;
                    border-bottom: 1px solid #e0e0e0 !important;
                    color: #333 !important;
                }}
                .footer {{
                    margin-top: 20px !important;
                    font-size: 14px !important;
                    color: #777 !important;
                    text-align: left !important;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>Refund Notification for Your Koi88 Order</h1>
                <p>Dear <span class='highlight'>{booking.Fullname}</span>,</p>
                <p>I hope this message finds you well. First and foremost, please accept our heartfelt apologies for the issue that occurred during the transportation of your koi order with Koi88. While we strive to ensure the safe delivery of every shipment, unfortunately, there was an incident that led to the loss of some koi.</p>
                <p>We deeply regret this situation and are committed to making things right to protect your interests. To compensate for this unforeseen issue, we will be refunding the deposit you paid for the affected koi. Additionally, as a gesture of goodwill and to make up for any inconvenience, we will provide additional compensation amounting to <strong>25%</strong> of the total order Koi value.</p>
                <p>To proceed with the refund and compensation, please provide us with your preferred <strong>payment method</strong> along with any necessary details so we can process this promptly.</p>
                <div class='table-container'>
                    <p><strong>Refund and Compensation Statement</strong></p>
                    <p>BookingID: <span class='highlight'>{booking.BookingId}</span></p>
                    <p>Trip Name: <span class='highlight'>{tripName}</span></p>
                    <table>
                        <tr>
                            <th>Item</th>
                            <th>Amount</th>
                        </tr>
                        <tr>
                            <td>Deposit to Farm:</td>
                            <td>{depositAmount:C} USD</td>
                        </tr>
                        <tr>
                            <td>Compensation for Loss (25%):</td>
                            <td>{compensationAmount:C} USD</td>
                        </tr>
                        <tr>
                            <td><strong>Total Refund:</strong></td>
                            <td><strong>{totalRefundAmount:C} USD</strong></td>
                        </tr>
                    </table>
                </div>
                <p>Once again, we sincerely apologize for any inconvenience this may have caused. We greatly value your business and are dedicated to resolving this situation in a manner that meets your expectations. If you have any further questions, please do not hesitate to reach out.</p>
                <p>Thank you very much for your understanding, and we look forward to hearing from you.</p>
                <div class='footer'>
                    <p>Warm regards,<br>Manager<br>Koi88</p>
                </div>
            </div>
        </body>
        </html>"
            };

            mailMessage.To.Add(email);



            try
            {
                using (var smtpClient = new SmtpClient(_smtpSettings.SmtpServer)
                {
                    Port = _smtpSettings.SmtpPort,
                    Credentials = new NetworkCredential(_smtpSettings.SmtpUser, _smtpSettings.SmtpPass),
                    EnableSsl = _smtpSettings.EnableSsl,
                })
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }


        private async Task SendLostDepositEmail(Booking booking)
        {
            // Đảm bảo truy vấn đầy đủ các liên kết từ Booking -> Po -> PoDetails -> Koi -> Farm
            var bookingData = _db.Bookings
                .Include(b => b.Po)
                .ThenInclude(po => po.Podetails)
                .ThenInclude(pd => pd.Koi)
                .Include(b => b.Po)
                .ThenInclude(po => po.Podetails)
                .ThenInclude(pd => pd.Farm)
                .FirstOrDefault(b => b.BookingId == booking.BookingId);

            if (bookingData == null) return;

            var email = bookingData.Email;
            var deliveryLocation = bookingData.Po?.DeliveryLocation ?? "Unknown location";
            var koiDeliveryDate = bookingData.Po?.KoiDeliveryDate;
            var koiDeliveryTime = bookingData.Po?.KoiDeliveryTime;

            // Chỉ tính tổng tiền cọc duy nhất một lần cho mỗi farm và ngày
            var totalDepositOverall = bookingData.Po?.Podetails?
                .GroupBy(pd => new { pd.Farm, pd.Day }) // Nhóm theo Farm và Ngày
                .Sum(group => group.FirstOrDefault()?.Deposit ?? 0) ?? 0; // Tính tổng cọc duy nhất

            // Nhóm thông tin theo FarmId và xử lý chi tiết về các Farm và cá Koi
            var farmDetails = bookingData.Po?.Podetails?
                .GroupBy(pd => new { pd.Farm, pd.Day }) // Nhóm theo Farm và Ngày đi
                .Select(farmGroup => new
                {
                    FarmName = farmGroup.Key.Farm?.FarmName ?? "Unknown farm",
                    Day = farmGroup.Key.Day?.ToString("dd/MM/yyyy") ?? "Unknown date", // Ngày đi trang trại từ PoDetail.Day
                    TotalDeposit = farmGroup.FirstOrDefault()?.Deposit ?? 0, // Lấy tổng cọc một lần cho mỗi farm với .FirstOrDefault()
                    KoiList = farmGroup.Select(pd => new
                    {
                        KoiName = pd.Koi?.KoiName ?? "Unknown Koi",
                        Quantity = pd.Quantity,
                        KoiPrice = pd.TotalKoiPrice // Sử dụng TotalKoiPrice
                    }).ToList()
                }).ToList();

            // Tạo nội dung bảng hiển thị chi tiết về các Farm, ngày và cá Koi với các cột riêng biệt
            var koiTableRows = string.Join("", farmDetails?.Select(farm => $@"
    <tr>
        <td>{farm.Day}</td> <!-- Cột Ngày đi trang trại -->
        <td>{farm.FarmName}</td>
        <td>{farm.TotalDeposit:C}</td> <!-- Hiển thị tổng cọc một lần cho mỗi farm -->
        <td>
            <table style='width:100%; border-collapse:collapse;'>
                <thead>
                    <tr>
                        <th style='border: 1px solid #e0e0e0; padding: 10px;'>Koi Fish</th>
                        <th style='border: 1px solid #e0e0e0; padding: 10px;'>Quantity</th>
                        <th style='border: 1px solid #e0e0e0; padding: 10px;'>Price (USD)</th>
                    </tr>
                </thead>
                <tbody>
                    {string.Join("", farm.KoiList.Select(koi => $@"
                    <tr>
                        <td style='border: 1px solid #e0e0e0; padding: 10px;'>{koi.KoiName}</td>
                        <td style='border: 1px solid #e0e0e0; padding: 10px;'>{koi.Quantity}</td>
                        <td style='border: 1px solid #e0e0e0; padding: 10px;'>{koi.KoiPrice:C}</td>
                    </tr>"))}
                </tbody>
            </table>
        </td>
    </tr>
") ?? new List<string>());

            // Nội dung email
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SmtpUser),
                Subject = "Notification Regarding the Delivery Status of Koi Order - Koi88 Order",
                IsBodyHtml = true,
                Body = $@"
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f7f9fc; color: #333; padding: 20px; }}
        .container {{ max-width: 600px; margin: auto; background: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); }}
        h1 {{ color: #007BFF; font-size: 24px; margin-bottom: 20px; text-align: center; }}
        p {{ font-size: 16px; line-height: 1.6; color: #555; margin-bottom: 15px; }}
        .highlight {{ color: #007BFF; font-weight: bold; }}
        .table-container {{ margin: 20px 0; font-size: 16px; }}
        table {{ width: 100%; border-collapse: collapse; border: 1px solid #e0e0e0; }}
        th, td {{ padding: 10px; border: 1px solid #e0e0e0; color: #333; text-align: center; }} /* Căn giữa nội dung */
        .footer {{ margin-top: 20px; font-size: 14px; color: #777; }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Koi Order Delivery Status ID: #{booking.BookingId}</h1>
        <p>Dear <span class='highlight'>{bookingData.Fullname}</span>,</p>
        <p>I am a Manager of the Koi transportation department at Koi88. We are writing this letter to inform you about the status of the delivery regarding your order.</p>
        <p>Per our agreed schedule, we arranged for delivery of the Koi fish to <span class='highlight'>{deliveryLocation}</span> on <span class='highlight'>{koiDeliveryDate}</span> at <span class='highlight'>{koiDeliveryTime}</span>. However, we received notification from our delivery team on the scheduled day that you either declined or refused to accept the delivery.</p>
        <p>As outlined in our policy, if a customer declines to accept a delivery on the agreed date, we retain the right to withhold 100% of the deposit paid to the farm, and a refund cannot be issued in this case.</p>
        <p>Please find below the details of your Koi fish purchase, including the purchase date, the farm name, and the total deposit for the order:</p>
        <div class='table-container'>
            <p><strong>Koi Purchase Details</strong></p>
            <p><strong>Trip Name: {booking.Trip?.TripName}</strong></p>
            <table>
                <thead>
                    <tr>
                        <th>Day</th>
                        <th>Farm</th>
                        <th>Deposit</th>
                        <th>Koi Fish Details</th>
                    </tr>
                </thead>
                <tbody>
                    {koiTableRows}
                </tbody>
            </table>
            <p><strong>Total Deposit:</strong> {totalDepositOverall} USD</p> <!-- Tổng cọc toàn bộ đơn hàng -->
        </div>
        <p>We sincerely hope for your cooperation and understanding to ensure that the delivery process proceeds smoothly and as planned. If you have any questions or need further assistance, please feel free to contact us at our hotline: <strong>0123-456-789</strong>.</p>
        <div class='footer'>
            <p>Sincerely,<br>Manager<br>Koi88</p>
        </div>
    </div>
</body>
</html>"
            };


            mailMessage.To.Add(email);

            // Gửi email
            try
            {
                using (var smtpClient = new SmtpClient(_smtpSettings.SmtpServer)
                {
                    Port = _smtpSettings.SmtpPort,
                    Credentials = new NetworkCredential(_smtpSettings.SmtpUser, _smtpSettings.SmtpPass),
                    EnableSsl = _smtpSettings.EnableSsl,
                })
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }











        public IActionResult KoiVarietyList(string query, int page = 1, int pageSize = 8)
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }
            // Start by fetching all varieties
            var varieties = _db.Varieties.AsQueryable();

            // If query is not null or empty, filter the varieties by VarietyName
            if (!string.IsNullOrEmpty(query))
            {
                var lowerCaseQuery = query.ToLower();
                varieties = varieties.Where(v => v.VarietyName.ToLower().Contains(lowerCaseQuery));
            }

            // Count total varieties after filtering
            var totalVarieties = varieties.Count();

            // Calculate total number of pages
            var totalPages = (int)Math.Ceiling((double)totalVarieties / pageSize);

            // Fetch the varieties for the current page using Skip and Take
            var varietiesOnCurrentPage = varieties
                .Select(v => new Variety
                {
                    VarietyId = v.VarietyId,
                    VarietyName = v.VarietyName,
                    ImageUrl = v.ImageUrl
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass current page, total pages, and search query to the view via ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Query = query;  // Pass query to keep it in the search input

            return View(varietiesOnCurrentPage);
        }


        public IActionResult CreateVariety()
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddVariety(Variety model, IFormFile ImageUrl, List<string> mainTopics, List<List<string>> subTopics)
        {
            if (ModelState.IsValid)
            {
                // Handle the image upload
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(ImageUrl.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(ImageUrl.FileName);
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/KoiVarieties");
                    var filePath = Path.Combine(directoryPath, fileName);

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageUrl.CopyTo(stream);
                    }

                    model.ImageUrl = "/images/KoiVarieties/" + fileName;
                }

                // Join the main topics list into a single string with '|' as delimiter
                model.MainTopics = string.Join("|", mainTopics);

                // Join each sub-topic list into a single string with ',' as delimiter, and join all lists with '|'
                model.SubTopics = string.Join("|", subTopics.Select(st => string.Join(",", st)));

                // Add the new variety to the database
                _db.Varieties.Add(model);
                _db.SaveChanges();

                // Redirect to the list of varieties after successful creation
                return Redirect("/Admin/Manager/KoiVarietyList");
            }

            // If model state is not valid, return the view with the current model
            return View(model);
        }


        [HttpPost]
        public IActionResult DeleteVariety(int id)
        {
            var variety = _db.Varieties.Find(id);
            if (variety != null)
            {
                _db.Varieties.Remove(variety);
                _db.SaveChanges();
            }

            // Redirect to the list of varieties
            return Redirect("/Admin/Manager/KoiVarietyList");
        }


        [HttpGet]
        public IActionResult UpdateVariety(int id)
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }
            var variety = _db.Varieties.Find(id);
            if (variety == null)
            {
                return NotFound();
            }
            return View(variety);
        }

        [HttpPost]
        public IActionResult UpdateVariety(int id, Variety model, IFormFile ImageUrl, List<string> MainTopic, List<List<string>> SubTopic)
        {
            // Find the existing variety by ID
            var existingVariety = _db.Varieties.FirstOrDefault(v => v.VarietyId == id);

            if (existingVariety == null)
            {
                return NotFound();
            }

            // Update the variety fields if provided
            if (!string.IsNullOrWhiteSpace(model.VarietyName))
            {
                existingVariety.VarietyName = model.VarietyName;
            }

            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                existingVariety.Description = model.Description;
            }

            // Handle image upload if a new image is provided
            if (ImageUrl != null && ImageUrl.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(ImageUrl.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(ImageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/KoiVarieties", fileName);

                // Save the new image to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageUrl.CopyTo(stream);
                }

                // Update the image URL in the database
                existingVariety.ImageUrl = "/images/KoiVarieties/" + fileName;
            }

            // Process Main Topics and Sub Topics into delimited strings
            if (MainTopic != null && MainTopic.Count > 0)
            {
                existingVariety.MainTopics = string.Join("|", MainTopic); // Join main topics with '|'
            }

            if (SubTopic != null && SubTopic.Count > 0)
            {
                var subTopicStrings = SubTopic.Select(stList => string.Join(",", stList)); // Join each subtopic list with ','
                existingVariety.SubTopics = string.Join("|", subTopicStrings); // Join all subtopic strings with '|'
            }

            // Save changes to the database
            _db.SaveChanges();

            // Redirect to the Update Variety page after a successful update
            return Redirect($"/Admin/Manager/UpdateVariety?id={id}");
        }


        public IActionResult KoiFishList(string query, int page = 1, int pageSize = 8)
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }
            // Start by fetching all koi fishes, including their Variety
            var koiFishes = _db.KoiFishes
                               .Include(k => k.Variety) // Include related Variety
                               .AsQueryable();

            // If there's a search query, filter by Koi Name or Variety Name
            if (!string.IsNullOrEmpty(query))
            {
                var lowerCaseQuery = query.ToLower(); // Convert to lowercase for case-insensitive search
                koiFishes = koiFishes.Where(k =>
                    k.KoiName.ToLower().Contains(lowerCaseQuery) ||  // Search in Koi Name
                    k.Variety.VarietyName.ToLower().Contains(lowerCaseQuery));  // Search in Variety Name
            }

            // Get total number of koi fishes after filtering
            var totalKoiFishes = koiFishes.Count();

            // Calculate total pages
            var totalPages = (int)Math.Ceiling((double)totalKoiFishes / pageSize);

            // Fetch koi fishes for the current page using Skip and Take
            var koiFishesOnCurrentPage = koiFishes
                                         .Skip((page - 1) * pageSize) // Skip previous pages
                                         .Take(pageSize)              // Take only koi fishes for the current page
                                         .ToList();

            // Pass current page, total pages, and search query to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Query = query; // Keep the query in the search box

            return View(koiFishesOnCurrentPage);
        }



        [HttpGet]
        public IActionResult CreateKoiFish()
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }
            // Fetch the list of varieties from the database
            var varieties = _db.Varieties.ToList();

            // Pass the list of varieties to the view using ViewBag
            ViewBag.Varieties = varieties;

            // Return the view for koi fish creation
            return View();
        }

        [HttpPost]
        public IActionResult AddKoiFish(KoiFish model, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                // Handle the image upload
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    // Create a unique file name to avoid overwriting files with the same name
                    var fileName = Path.GetFileNameWithoutExtension(ImageUrl.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(ImageUrl.FileName);
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/KoiFish");
                    var filePath = Path.Combine(directoryPath, fileName);

                    // Ensure the directory exists
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageUrl.CopyTo(stream);
                    }

                    // Set the image URL in the model (relative path)
                    model.ImageUrl = "/images/KoiFish/" + fileName;
                }

                // Add the new variety to the database
                _db.KoiFishes.Add(model);
                _db.SaveChanges();


                return Redirect("/Admin/Manager/KoiFishList");
            }


            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteKoiFish(int id)
        {
            var koiFishes = _db.KoiFishes.Find(id);
            if (koiFishes != null)
            {
                _db.KoiFishes.Remove(koiFishes);
                _db.SaveChanges();
            }

            // Redirect to the list of varieties
            return Redirect("/Admin/Manager/KoiFishList");
        }

        public IActionResult UpdateKoiFish(int id)
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }
            var koiFish = _db.KoiFishes.Find(id);
            if (koiFish == null)
            {
                return NotFound();
            }

            // Fetch the list of varieties to populate the dropdown
            var varieties = _db.Varieties.ToList();

            // Prepare a ViewModel (or use ViewBag) to pass data to the view
            ViewBag.Varieties = varieties;
            return View(koiFish);
        }

        [HttpPost]
        public IActionResult EditKoiFish(int id, KoiFish model, IFormFile ImageUrl)
        {

            var existingKoiFish = _db.KoiFishes.FirstOrDefault(k => k.KoiId == id);

            if (existingKoiFish == null)
            {
                return NotFound();
            }


            if (!string.IsNullOrWhiteSpace(model.KoiName))
            {
                existingKoiFish.KoiName = model.KoiName;
            }

            if (!string.IsNullOrWhiteSpace(model.Koinamejp))
            {
                existingKoiFish.Koinamejp = model.Koinamejp;
            }

            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                existingKoiFish.Description = model.Description;
            }

            if (!string.IsNullOrWhiteSpace(model.Size))
            {
                existingKoiFish.Size = model.Size;
            }

            if (!string.IsNullOrWhiteSpace(model.Age))
            {
                existingKoiFish.Age = model.Age;
            }

            if (!string.IsNullOrWhiteSpace(model.Price))
            {
                existingKoiFish.Price = model.Price;
            }



            // Cập nhật VarietyId và VarietyName
            if (model.VarietyId != 0)
            {
                existingKoiFish.VarietyId = model.VarietyId;

                // Cập nhật VarietyName từ variety model
                var variety = _db.Varieties.FirstOrDefault(v => v.VarietyId == model.VarietyId);
                if (variety != null)
                {
                    existingKoiFish.Variety.VarietyName = variety.VarietyName;
                }
            }


            if (ImageUrl != null && ImageUrl.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(ImageUrl.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(ImageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/KoiFish", fileName);

                // Lưu ảnh mới vào server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageUrl.CopyTo(stream);
                }

                // Cập nhật URL ảnh trong cơ sở dữ liệu với ảnh mới
                existingKoiFish.ImageUrl = "/images/KoiFish/" + fileName;
            }


            _db.SaveChanges();


            return Redirect($"/Admin/Manager/UpdateKoiFish?id={id}");
        }

        public IActionResult KoiFarmList(string query, int page = 1, int pageSize = 5)
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }
            // Bắt đầu với danh sách tất cả các farm
            var koiFarms = _db.KoiFarms
                .Include(farm => farm.SpecialVarieties)
                .ThenInclude(sv => sv.Variety)
                .AsQueryable();

            // Nếu query không rỗng hoặc null, tiến hành tìm kiếm
            if (!string.IsNullOrEmpty(query))
            {
                var lowerCaseQuery = query.ToLower();
                koiFarms = koiFarms.Where(farm =>
                    farm.FarmName.ToLower().Contains(lowerCaseQuery) ||
                    farm.Location.ToLower().Contains(lowerCaseQuery) ||
                    farm.SpecialVarieties.Any(sv => sv.Variety.VarietyName.ToLower().Contains(lowerCaseQuery))
                );
            }

            // Tính tổng số farm sau khi lọc
            var totalFarms = koiFarms.Count();

            // Tính toán số trang
            var totalPages = (int)Math.Ceiling((double)totalFarms / pageSize);

            // Sử dụng Skip và Take để lấy đúng dữ liệu cho trang hiện tại
            var farmsOnCurrentPage = koiFarms
                .Skip((page - 1) * pageSize) // Skip các farm của các trang trước
                .Take(pageSize)              // Chỉ lấy đúng số lượng farm cho trang hiện tại
                .ToList();

            // Trả về View với dữ liệu đã phân trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(farmsOnCurrentPage);
        }

        [HttpGet]
        public IActionResult CreateFarm()
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }

            var varieties = _db.Varieties.ToList();


            ViewBag.Varieties = varieties;


            return View();
        }
        [HttpPost]
        public IActionResult AddKoiFarm(KoiFarm model, IFormFile imageUrl, List<int> selectedVarietyIds)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (imageUrl != null && imageUrl.Length > 0)
                {
                    // Create a unique file name to avoid overwriting
                    var fileName = Path.GetFileNameWithoutExtension(imageUrl.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(imageUrl.FileName);
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/KoiFarm");
                    var filePath = Path.Combine(directoryPath, fileName);

                    // Create directory if it does not exist
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Save the image file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageUrl.CopyTo(stream);
                    }

                    model.ImageUrl = Url.Content("~/images/KoiFarm/" + fileName); // Use Url.Content to handle URL properly
                }

                
                _db.KoiFarms.Add(model);
                _db.SaveChanges(); 

                // Create SpecialVarieties based on selected varieties
                if (selectedVarietyIds != null && selectedVarietyIds.Count > 0)
                {
                    foreach (var varietyId in selectedVarietyIds)
                    {
                        var specialVariety = new SpecialVariety
                        {
                            FarmId = model.FarmId, // Assuming FarmId is generated after SaveChanges
                            VarietyId = varietyId
                        };
                        _db.SpecialVarieties.Add(specialVariety);
                    }
                    _db.SaveChanges();

                return Redirect("/Admin/Manager/KoiFarmList");
            }

            // If the model state is invalid, return to the create view with the current model
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteKoiFarm(int id)
        {
            var farmToDelete = _db.KoiFarms
                .Include(f => f.SpecialVarieties) // Include related SpecialVarieties
                .FirstOrDefault(f => f.FarmId == id);

            if (farmToDelete == null)
            {
                return NotFound();
            }

            // Remove associated SpecialVariety records first
            _db.SpecialVarieties.RemoveRange(farmToDelete.SpecialVarieties);

       
            _db.KoiFarms.Remove(farmToDelete);

            _db.SaveChanges();

            return Redirect("/Admin/Manager/KoiFarmList");
        }


        [HttpGet]
        public IActionResult UpdateFarm(int id)
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }
            // Fetch the KoiFarm with its associated SpecialVarieties
            var model = _db.KoiFarms
                .Include(farm => farm.SpecialVarieties)
                .ThenInclude(sv => sv.Variety)
                .FirstOrDefault(farm => farm.FarmId == id);

            if (model == null)
            {
                return NotFound();
            }

            // Get the list of varieties for the checkbox list
            var varieties = _db.Varieties.ToList();
            ViewBag.Varieties = varieties;

            return View(model);
        }

        [HttpPost]
        public IActionResult EditKoiFarms(int id, KoiFarm model, IFormFile ImageUrl, int[] selectedVarietyIds)
        {
            // Fetch the existing KoiFarm from the database
            var existingKoiFarm = _db.KoiFarms
                .Include(farm => farm.SpecialVarieties)
                .ThenInclude(sv => sv.Variety)
                .FirstOrDefault(farm => farm.FarmId == id);

            if (existingKoiFarm == null)
            {
                return NotFound();
            }

            // Update properties of the existing KoiFarm
            if (!string.IsNullOrWhiteSpace(model.FarmName))
            {
                existingKoiFarm.FarmName = model.FarmName;
            }

            if (!string.IsNullOrWhiteSpace(model.Location))
            {
                existingKoiFarm.Location = model.Location;
            }

            if (!string.IsNullOrWhiteSpace(model.ContactInfo))
            {
                existingKoiFarm.ContactInfo = model.ContactInfo;
            }

            // Convert selectedVarietyIds to a list for easier manipulation
            var selectedVarietiesList = selectedVarietyIds?.ToList() ?? new List<int>();

            // Update VarietyId for existing SpecialVarieties
            foreach (var specialVariety in existingKoiFarm.SpecialVarieties.ToList())
            {
                if (selectedVarietiesList.Contains((int)specialVariety.VarietyId))
                {
                    // If it is, keep it as is (no need to change)
                    continue;
                }
                else
                {
                    // If it's not in the selected variety IDs, you may want to remove it
                    _db.SpecialVarieties.Remove(specialVariety);
                }
            }

            // If you want to add new special varieties based on the selected IDs
            foreach (var selectedVarietyId in selectedVarietiesList)
            {
                // Check if the special variety already exists
                if (!existingKoiFarm.SpecialVarieties.Any(sv => sv.VarietyId == selectedVarietyId))
                {
                    // If it doesn't exist, create a new SpecialVariety
                    existingKoiFarm.SpecialVarieties.Add(new SpecialVariety
                    {
                        FarmId = existingKoiFarm.FarmId,
                        VarietyId = selectedVarietyId
                    });
                }
            }

            // Handle image upload if provided
            if (ImageUrl != null && ImageUrl.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(ImageUrl.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(ImageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/KoiFarm", fileName);

                // Save the new image to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageUrl.CopyTo(stream);
                }

                // Update the ImageUrl in the KoiFarm
                existingKoiFarm.ImageUrl = "/images/KoiFarm/" + fileName;
            }

         
            _db.SaveChanges();

          
            return Redirect($"/Admin/Manager/UpdateFarm?id={id}");
        }

        public async Task<IActionResult> Feedback(int page = 1, int pageSize = 8)
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }
            // Query the feedbacks with related customer and trip information
            var feedbacks = await _db.Feedbacks
                                     .Include(f => f.Customer)
                                     .Include(f => f.Bookings)
                                     .ThenInclude(b => b.Trip)
                                     .OrderByDescending(f => f.Feedbackdate)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

            // Get the total number of feedbacks for pagination
            var totalFeedbacks = await _db.Feedbacks.CountAsync();

            // Send feedbacks and pagination info to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalFeedbacks / (double)pageSize);

            return View(feedbacks);
        }


        public IActionResult FeedbackDetail(int feedbackId)
        {
            var adminRoleId = HttpContext.Session.GetInt32("AdminRoleId");

            if (adminRoleId != 2)
            {
                return RedirectToAction("Unauthorized", "Home");
            }
            var feedback = _db.Feedbacks
                .Include(f => f.Bookings)
                .ThenInclude(b => b.Trip)
                .FirstOrDefault(f => f.FeedbackId == feedbackId);

            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback); 
        }




    }
}







