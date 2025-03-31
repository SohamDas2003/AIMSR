using AIMSR.Data;
using AIMSR.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity with Roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Create default Admin role and user
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Create roles
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    if (!await roleManager.RoleExistsAsync("Student"))
    {
        await roleManager.CreateAsync(new IdentityRole("Student"));
    }
    
    // Create admin user
    var adminUser = await userManager.FindByEmailAsync("admin@aimsr.edu.in");
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = "admin@aimsr.edu.in",
            Email = "admin@aimsr.edu.in",
            FullName = "System Administrator",
            EmailConfirmed = true
        };
        
        await userManager.CreateAsync(adminUser, "Admin@123");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
    
    // Clear any sample data for non-admin users
    try
    {
        var nonAdminUsers = await userManager.GetUsersInRoleAsync("Student");
        foreach (var user in nonAdminUsers)
        {
            if (int.TryParse(user.StudentId, out int rollNo))
            {
                // Remove all attendance records
                var attendanceRecords = dbContext.Attendance.Where(a => a.RollNo == rollNo).ToList();
                if (attendanceRecords.Any())
                {
                    dbContext.Attendance.RemoveRange(attendanceRecords);
                }
                
                // Remove all marks records
                var marksRecords = dbContext.Marks.Where(m => m.RollNo == rollNo).ToList();
                if (marksRecords.Any())
                {
                    dbContext.Marks.RemoveRange(marksRecords);
                }
            }
        }
        
        await dbContext.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        // Log the exception but don't crash the app
        Console.WriteLine($"Error clearing sample data: {ex.Message}");
    }
}

app.Run();
