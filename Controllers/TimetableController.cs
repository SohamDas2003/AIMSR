using Microsoft.AspNetCore.Mvc;

namespace AIMSR.Controllers
{
    

    public class TimetableController : Controller
    {
        public IActionResult Index()
        {
            return View("Timetable"); // Ensure it matches the view file name
        }
    }

}
