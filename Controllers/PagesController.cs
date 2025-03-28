using Microsoft.AspNetCore.Mvc;

namespace AIMSR.Controllers
{
    public class PagesController : Controller
    {
        public IActionResult Index(string page)
        {
            if (string.IsNullOrEmpty(page))
                return View("Error"); // Redirect to an error page if no page is specified

            return View(page);
        }
    }
}
