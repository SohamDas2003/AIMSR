using Microsoft.AspNetCore.Mvc;
using AIMSR.Models;

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

        [HttpPost]
        public IActionResult Submit(AdmissionForm model)
        {
            // In a real-world scenario, we would:
            // 1. Validate the model
            // 2. Save the submission to a database
            // 3. Maybe send a notification email
            
            // For now, we'll just redirect to the success page
            // We can pass the name to personalize the success message
            return RedirectToAction("SubmissionSuccess", new { name = model.FullName });
        }

        public IActionResult SubmissionSuccess(string name)
        {
            ViewBag.Name = name;
            return View();
        }
    }
}
