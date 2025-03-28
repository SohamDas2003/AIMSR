using Microsoft.AspNetCore.Mvc;
using AIMSR.Data;
using AIMSR.Models;
using System.Linq;

namespace AIMSR.Controllers
{
    public class FeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult DisplayFee()
        {
            // Get the first record from the database
            var singleFee = _context.Fees.FirstOrDefault();

            if (singleFee == null)
            {
                return NotFound(); // Handle case where no record exists
            }

            return View(singleFee); // Pass a single Fees object
        }
    }
}
