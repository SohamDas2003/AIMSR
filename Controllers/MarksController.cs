using Microsoft.AspNetCore.Mvc;

namespace AIMSR.Controllers
{

    public class MarksController : Controller
    {
        public IActionResult Index()
        {
            return View("Marks"); // Ensure it matches the view file name
        }
    }
}