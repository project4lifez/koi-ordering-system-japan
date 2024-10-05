using Microsoft.EntityFrameworkCore; // Required for UseSqlServer
using KoiOrderingSystem.Models; // Ensure this is the correct namespace for Koi88Context

namespace KoiOrderingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register the database context (assuming SQL Server)
            // Make sure "DefaultConnection" matches your connection string in appsettings.json
            builder.Services.AddDbContext<Koi88Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Enable session state
            builder.Services.AddDistributedMemoryCache(); // For session storage
            builder.Services.AddSession();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // HSTS configuration
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Enable session and add authentication if needed
            app.UseSession();

            app.UseAuthorization();

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
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Manager}/{action=Manager}/{id?}");
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Consulting}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Sale}/{action=Index}/{id?}");
            app.MapControllerRoute(
               name: "areas",
               pattern: "{area:exists}/{controller=Delivering}/{action=Index}/{id?}");
            app.MapControllerRoute(
               name: "areas",
               pattern: "{area:exists}/{controller=Sale}/{action=Quote}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Customer}/{action=YourOrder}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Customer}/{action=TripDetails}/{id?}");
            app.Run();
        }
    }
}
