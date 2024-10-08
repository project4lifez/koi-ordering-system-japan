using KoiOrderingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

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
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add Google authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = "#"; // Replace with your ClientId
                googleOptions.ClientSecret = "#"; // Replace with your ClientSecret
                googleOptions.CallbackPath = "/signin-google"; // This is the path Google will redirect to after authentication
            });

            // Add localization support
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Configure supported cultures (English and Japanese)
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"), // English
                    new CultureInfo("ja")  // Japanese
                };
                options.DefaultRequestCulture = new RequestCulture("en"); // Default is English
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

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

            // Add localization middleware
            var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);

            // Map the default routes
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
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Home}/{id?}");

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Manager}/{action=Manager}/{id?}");

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Consulting}/{action=Consulting}/{id?}");

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Sale}/{action=Sale}/{id?}");

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Sale}/{action=UpdateQuote}/{id?}");

            app.MapControllerRoute(
               name: "areas",
               pattern: "{area:exists}/{controller=Delivering}/{action=Delivering}/{id?}");

            app.MapControllerRoute(
               name: "areas",
               pattern: "{area:exists}/{controller=Home}/{action=OrderManagement}/{id?}");

            app.Run();
        }
    }
}
