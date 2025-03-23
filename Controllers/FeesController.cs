using Microsoft.AspNetCore.Mvc;

namespace AIMSR.Controllers;

public class FeesController : Controller
{
    public IActionResult Index()
    {
        return View("Fees");
    }

    public IActionResult Pay()
    {
        return View("Pay");
    }
}
