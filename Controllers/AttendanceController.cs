using Microsoft.AspNetCore.Mvc;

namespace AIMSR.Controllers
{

    public class AttendanceController : Controller
    {
        public IActionResult Index()
        {
            return View("Attendance"); // Ensure it matches the view file name
        }
    }
}

