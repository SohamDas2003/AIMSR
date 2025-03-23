using Microsoft.AspNetCore.Mvc;

namespace AIMSR.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            ViewData["StudentName"] = "Soham Das";
            ViewData["RollNumber"] = "MCA2025001";
            ViewData["Email"] = "soham.das@example.com";
            ViewData["Course"] = "Master of Computer Applications";
            ViewData["Year"] = "1st Year";
            ViewData["Department"] = "Computer Science";

            return View("Profile");
        }
    }
}