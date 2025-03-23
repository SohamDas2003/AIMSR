using Microsoft.AspNetCore.Mvc;

namespace AIMSR.Controllers
{

    public class LibraryController : Controller
    {
        public IActionResult Index()
        {
            return View("Library"); // Ensure it matches the view file name
        }
    }

}
