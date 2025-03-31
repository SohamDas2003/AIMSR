using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AIMSR.Models;
using System.Threading.Tasks;

namespace AIMSR.Controllers
{
    [Authorize] // Require authentication for all actions in this controller
    public class StudentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Get current user
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null)
            {
                return NotFound("User not found");
            }
            
            // Create view model from user data
            var viewModel = new StudentViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                StudentId = user.StudentId,
                Course = user.Course,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
            };
            
            return View(viewModel);
        }
    }
} 