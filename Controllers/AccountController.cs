using AIMSR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AIMSR.Data;
using System.Linq;

namespace AIMSR.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, 
                               SignInManager<ApplicationUser> signInManager,
                               ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Validate the StudentId is not already taken
                var existingUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.StudentId == model.StudentId);
                
                if (existingUser != null)
                {
                    ModelState.AddModelError("StudentId", "This Student ID/Roll No is already in use.");
                    return View(model);
                }
                
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    StudentId = model.StudentId,
                    Course = model.Course,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Assign Student role by default
                    await _userManager.AddToRoleAsync(user, "Student");
                    
                    // Redirect to login page after successful registration
                    return RedirectToAction("Login");
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                 var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Student");
                }
                
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Logout (for menu link)
        public async Task<IActionResult> LogoutGet()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ClearMyData()
        {
            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }
            
            // Make sure this is a student
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Student"))
            {
                return Forbid("Only students can clear their data");
            }
            
            if (int.TryParse(user.StudentId, out int rollNo))
            {
                // Remove all attendance records
                var attendanceRecords = _context.Attendance.Where(a => a.RollNo == rollNo).ToList();
                if (attendanceRecords.Any())
                {
                    _context.Attendance.RemoveRange(attendanceRecords);
                }
                
                // Remove all marks records
                var marksRecords = _context.Marks.Where(m => m.RollNo == rollNo).ToList();
                if (marksRecords.Any())
                {
                    _context.Marks.RemoveRange(marksRecords);
                }
                
                await _context.SaveChangesAsync();
                
                TempData["Message"] = "All your data has been cleared successfully.";
            }
            
            return RedirectToAction("Index", "Student");
        }
    }
} 