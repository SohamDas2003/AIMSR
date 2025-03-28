using Microsoft.AspNetCore.Mvc;

namespace AIMSR.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View("About");
        }
    }
}
