using DocumentFormat.OpenXml.Spreadsheet;
using KoiOrderingSystem.Models; // Import your model namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For database operations like FirstOrDefaultAsync
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace KoiOrderingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly Koi88Context _db; // Injecting the database context
        private readonly SmtpSettings _smtpSettings;
        private static string generatedOtp;
        private static string emailForOtp;

        public AccountController(Koi88Context db, IOptions<SmtpSettings> smtpSettings)
        {
            _db = db; // Assigning the database context
            _smtpSettings = smtpSettings.Value; // Assigning SMTP settings
        }

        // Step 1: Forgot password (Send OTP)
        private static DateTime? lastOtpSent; // Add this field to keep track of the last OTP sent time

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOtp(string email)
        {
            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email);

            if (account == null)
            {
                ViewBag.Error = "Email does not exist in the system.";
                return View("ForgotPassword");
            }

            // Check if the last OTP was sent less than 60 seconds ago
            if (lastOtpSent.HasValue && (DateTime.Now - lastOtpSent.Value).TotalSeconds < 60)
            {
                ViewBag.Error = "Please wait before requesting a new OTP.";
                return View("VerifyOtp");
            }

            // Generate and send OTP
            var rand = new Random();
            generatedOtp = rand.Next(100000, 999999).ToString();
            emailForOtp = email;

            await SendOtpEmail(email, generatedOtp);

            lastOtpSent = DateTime.Now; // Update last OTP sent time

            ViewBag.Message = "A new OTP has been sent to your email.";
            ViewBag.Email = email;
            return View("VerifyOtp");
        }


        // Step 2: OTP Verification
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOtp(string otp)
        {
            // Store the email in ViewBag before checking the OTP

            // Check if the provided OTP is valid
            if (otp != generatedOtp)
            {
                ViewBag.Error = "Invalid OTP. Please try again.";
                ViewBag.Email = emailForOtp; // Use the variable that holds the email used for OTP

                return View(); // Return the view with error message and email
            }

            // OTP verified, allow user to reset password
            return RedirectToAction("ResetPassword"); // Redirect to the reset password action
        }

        // Step 3: Reset Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string newPassword, string confirmPassword)
        {
            var errors = new List<string>();

            if (newPassword != confirmPassword)
            {
                errors.Add("New password and confirm password do not match.");
            }

            if (newPassword.Length < 8)
            {
                errors.Add("New password must be at least 8 characters long.");
            }

            if (errors.Any())
            {
                ViewBag.Errors = errors;
                return View();
            }

            // Find the account by the stored email (emailForOtp)
            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == emailForOtp);
            if (account == null)
            {
                ViewBag.Errors = new List<string> { "Account not found." };
                return View();
            }

            // Update the password in the database
            account.Password = newPassword; // Note: It's recommended to hash passwords in production.
            await _db.SaveChangesAsync();

            // Redirect to login page after successful password reset
            return RedirectToAction("", "Login");
        }

        // Method to send OTP via email
        private async Task SendOtpEmail(string email, string otp)
        {
            try
            {
                var smtpClient = new SmtpClient(_smtpSettings.SmtpServer)
                {
                    Port = _smtpSettings.SmtpPort,
                    Credentials = new NetworkCredential(_smtpSettings.SmtpUser, _smtpSettings.SmtpPass),
                    EnableSsl = _smtpSettings.EnableSsl,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.SmtpUser),
                    Subject = "Your OTP Code",
                    IsBodyHtml = true,
                    Body = $@"
    <html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f7f9fc;
                padding: 20px;
            }}
            .container {{
                max-width: 600px;
                margin: auto;
                background: #ffffff;
                padding: 30px;
                border-radius: 10px;
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                border: 1px solid #e0e0e0;
            }}
            h2 {{
                color: #333;
                text-align: center; /* Center the heading */
                margin-bottom: 20px;
            }}
            .otp-code {{
                font-size: 36px; /* Increased font size for better visibility */
                font-weight: bold;
                color: #007BFF;
                padding: 15px;
                border: 2px dashed #007BFF;
                display: inline-block;
                margin: 20px 0;
                text-align: center; /* Center the OTP code */
                border-radius: 5px;
                background-color: #f1f8ff;
            }}
            p {{
                font-size: 16px;
                line-height: 1.5;
                color: #555;
                text-align: left; /* Left align paragraph text */
            }}
            .footer {{
                margin-top: 20px;
                font-size: 14px;
                color: #777;
                text-align: left; /* Left align footer text */
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <h2>Your OTP Code</h2>
            <p>Dear User,</p>
            <p>Thank you for using <strong>Koi88</strong>! Your OTP code is:</p>
            <div class='otp-code'>{otp}</div>
            <p>Please enter this code to verify your identity.</p>
            <p>If you did not request this code, please ignore this email.</p>
            <div class='footer'>
                <p>Best Regards,<br>Koi88 Team</p>
            </div>
        </div>
    </body>
    </html>"
                };



                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
                ViewBag.Email = email;

            }
            catch (Exception ex)
            {
                // Log or display the error message
                Console.WriteLine($"Error sending email: {ex.Message}");
                // You can also add logic to return this error to the view
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOtp([FromBody] string email)
        {
            // Check if the email exists in the database
            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email);

            if (account == null)
            {
                return BadRequest("Email does not exist in the system."); // Return a bad request if the email does not exist
            }

            // Generate new OTP
            var rand = new Random();
            generatedOtp = rand.Next(100000, 999999).ToString(); // 6-digit OTP
            emailForOtp = email; // Use the passed email for OTP

            // Send new OTP via email
            await SendOtpEmail(email, generatedOtp);

            return Ok("A new OTP has been sent to your email."); // Return a success message
        }






        // GET: Forgot Password
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // GET: Verify OTP
        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }

        // GET: Reset Password
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }


    }
}
