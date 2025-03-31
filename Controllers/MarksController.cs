using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AIMSR.Data;
using AIMSR.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AIMSR.Controllers
{
    [Authorize]
    public class MarksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MarksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Get current logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }
            
            // FOR NEW USERS: Always return empty list to ensure no mock data is shown
            // We will force this behavior until an admin specifically adds marks records
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
            {
                var isNewUser = DateTime.UtcNow.Subtract(
                    user.LockoutEnd?.UtcDateTime ?? DateTime.MinValue
                ).TotalDays < 7;
                if (isNewUser)
                {
                    return View(new List<Marks>());
                }
            }

            // Get student roll number
            if (!int.TryParse(user.StudentId, out int rollNo))
            {
                return View(new List<Marks>()); // No marks yet
            }

            // Get marks for the student
            var marks = await _context.Marks
                .Where(m => m.RollNo == rollNo)
                .OrderByDescending(m => m.ExamDate)
                .ToListAsync();

            // Return marks (will be empty for new users)
            return View(marks);
        }
    }
}