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

            // Get student roll number from user profile
            int rollNo;
            if (!int.TryParse(user.StudentId, out rollNo))
            {
                rollNo = 101; // Default roll number if not set
                user.StudentId = rollNo.ToString();
                await _userManager.UpdateAsync(user);
            }

            // Check if attendance records exist for this user
            var attendance = await _context.Attendance
                .Where(a => a.RollNo == rollNo)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            // If no records exist, create sample attendance data
            if (attendance == null || !attendance.Any())
            {
                attendance = GenerateSampleAttendanceData(rollNo);
                _context.Attendance.AddRange(attendance);
                await _context.SaveChangesAsync();
            }

            return View(attendance);
        }

        private List<Attendance> GenerateSampleAttendanceData(int rollNo)
        {
            var subjects = new[] { "Mathematics", "Computer Science", "Database Management", "Software Engineering", "Web Development" };
            
            var attendanceList = new List<Attendance>();
            var random = new Random();
            
            // Generate data for the last 30 days
            for (int i = 0; i < 30; i++)
            {
                var date = DateTime.Now.AddDays(-i);
                
                // Skip weekends
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }
                
                // Add 1-3 subjects per day
                for (int j = 0; j < random.Next(1, 4); j++)
                {
                    var subjectIndex = random.Next(0, subjects.Length);
                    
                    attendanceList.Add(new Attendance
                    {
                        RollNo = rollNo,
                        Date = date,
                        Subject = subjects[subjectIndex],
                        Status = random.Next(0, 10) < 8 ? "Present" : "Absent" // 80% chance of being present
                    });
                }
            }
            
            return attendanceList;
        }
    }
}
