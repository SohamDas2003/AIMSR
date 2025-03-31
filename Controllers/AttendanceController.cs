using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AIMSR.Data;
using AIMSR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIMSR.Controllers
{
    [Authorize] // Require authentication
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttendanceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // FOR NEW USERS: Always return empty list to ensure no mock data is shown
            // We will force this behavior until an admin specifically adds attendance records
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
            {
                var isNewUser = DateTime.UtcNow.Subtract(
                    user.LockoutEnd?.UtcDateTime ?? DateTime.MinValue
                ).TotalDays < 7;
                if (isNewUser)
                {
                    return View(new List<Attendance>());
                }
            }

            // Get student roll number from user profile
            int rollNo;
            if (!int.TryParse(user.StudentId, out rollNo))
            {
                return View(new List<Attendance>()); // Return empty list if roll number is not valid
            }

            // Get attendance records for this user
            var attendance = await _context.Attendance
                .Where(a => a.RollNo == rollNo)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(attendance);
        }
    }
}
