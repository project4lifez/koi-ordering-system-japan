using KoiOrderingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using PayPal.Api;

namespace KoiOrderingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register the database context (SQL Server)
            builder.Services.AddDbContext<Koi88Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Enable session state
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Register PayPal configuration
            var payPalConfig = builder.Configuration.GetSection("PayPal");
            builder.Services.Configure<PayPalConfig>(payPalConfig);

            // Register SMTP settings for email (OTP)
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("EmailSettings"));

            // Add Google authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                // Custom cookie configuration (optional)
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/Login"; // Redirect here if unauthorized
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = "#"; // Replace with your ClientId
                googleOptions.ClientSecret = "#"; // Replace with your ClientSecret
                googleOptions.CallbackPath = "/signin-google"; // Path where Google redirects after authentication
            });

            // Register controllers with views
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Enable session and add authentication
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // Define general routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=HomePage}/{id?}");

            app.MapControllerRoute(
                name: "login_default",
                pattern: "{controller=Login}/{action=Login}/{id?}");

            app.MapControllerRoute(
                name: "register_default",
                pattern: "{controller=Register}/{action=Register}/{id?}");

            app.MapControllerRoute(
                name: "YourBooking_default",
                pattern: "{controller=YourBooking}/{action=YourBooking}/{id?}");

            app.MapControllerRoute(
               name: "ForgotPasword_default",
               pattern: "{controller=Account}/{action=ForgotPassword}/{id?}");

            app.MapControllerRoute(
               name: "Farm_default",
               pattern: "{controller=Farm}/{action=Farm}/{id?}");

            app.MapControllerRoute(
              name: "Farm_default",
              pattern: "{controller=Variety}/{action=Variety}/{id?}");

            // Add a specific route for PaymentExecuted
            app.MapControllerRoute(
                name: "YourBookingPaymentExecuted",
                pattern: "YourBooking/PaymentExecuted/{paymentId}/{token}/{PayerID}/{bookingId?}",
                defaults: new { controller = "YourBooking", action = "PaymentExecuted" });

            app.MapControllerRoute(
                name: "Profile_default",
                pattern: "{controller=Profile}/{action=CustomerProfile}/{id?}");

            // Area-based routing for better organization
            app.MapControllerRoute(
                name: "manager_area",
                pattern: "{area:exists}/{controller=Manager}/{action=Home}/{id?}");

            app.MapControllerRoute(
                name: "consulting_area",
                pattern: "{area:exists}/{controller=Consulting}/{action=Consulting}/{id?}");

            app.MapControllerRoute(
                name: "sale_area",
                pattern: "{area:exists}/{controller=Sale}/{action=Sale}/{id?}");

            app.MapControllerRoute(
                name: "sale_quote_update",
                pattern: "{area:exists}/{controller=Sale}/{action=UpdateQuote}/{id?}");

            app.MapControllerRoute(
                name: "delivering_area",
                pattern: "{area:exists}/{controller=Delivering}/{action=Delivering}/{id?}");

            app.MapControllerRoute(
                name: "order_management_area",
                pattern: "{area:exists}/{controller=Home}/{action=OrderManagement}/{id?}");

            app.MapControllerRoute(
                name: "order_management_list",
                pattern: "{area:exists}/{controller=Home}/{action=OrderList}/{id?}");

            app.MapControllerRoute(
               name: "podetail_area",
               pattern: "{area:exists}/{controller=PoDetail}/{action=PoDetail}/{id?}");

            app.Run();
        }
    }
}
