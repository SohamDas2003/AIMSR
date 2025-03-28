using Microsoft.AspNetCore.Mvc;
using AIMSR.Data;
using System.Linq;

namespace AIMSR.Controllers
{
    [Route("Attendance")] // Set route prefix
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{rollNo}")] // Define parameter in route
        public IActionResult GetAttendance(int rollNo)
        {
            var attendance = _context.Attendance
                .Where(a => a.RollNo == rollNo)
                .OrderByDescending(a => a.Date)
                .ToList();

            if (attendance == null || !attendance.Any())
            {
                return NotFound("No attendance records found.");
            }

            return View("Attendance", attendance); // Ensure Attendance.cshtml exists in Views/Attendance
        }
    }
}
