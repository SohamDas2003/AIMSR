using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AIMSR.Data;
using AIMSR.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AIMSR.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext context, 
                              UserManager<ApplicationUser> userManager,
                              RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Role Management

        public async Task<IActionResult> Roles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("", "Role name cannot be empty");
                return View();
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                ModelState.AddModelError("", "Role already exists");
                return View();
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                return RedirectToAction("Roles");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }

        #endregion

        #region User Management

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = string.Join(", ", roles)
                });
            }

            return View(userViewModels);
        }

        public async Task<IActionResult> EditUserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName
            };

            var roles = await _roleManager.Roles.ToListAsync();
            foreach (var role in roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await _userManager.IsInRoleAsync(user, role.Name ?? string.Empty))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Roles.Add(userRolesViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRoles(EditUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove existing roles");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user, 
                model.Roles.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles");
                return View(model);
            }

            return RedirectToAction("Users");
        }

        #endregion

        #region Attendance Management

        public async Task<IActionResult> ManageAttendance()
        {
            var users = await _userManager.Users.ToListAsync();
            var attendanceViewModel = new List<AttendanceManagementViewModel>();

            foreach (var user in users)
            {
                if(!int.TryParse(user.StudentId, out int rollNo))
                {
                    continue;
                }

                var attendance = await _context.Attendance
                    .Where(a => a.RollNo == rollNo)
                    .OrderByDescending(a => a.Date)
                    .ToListAsync();

                attendanceViewModel.Add(new AttendanceManagementViewModel
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    RollNo = rollNo,
                    Course = user.Course,
                    AttendanceRecords = attendance
                });
            }

            return View(attendanceViewModel);
        }

        public async Task<IActionResult> CreateAttendance()
        {
            // Populate students dropdown
            var students = await _userManager.GetUsersInRoleAsync("Student");
            ViewBag.Students = new SelectList(students
                .Where(s => !string.IsNullOrEmpty(s.StudentId))
                .Select(s => new 
                {
                    StudentId = s.StudentId,
                    DisplayName = $"{s.FullName} ({s.StudentId})"
                }), "StudentId", "DisplayName");

            // Populate subjects dropdown
            ViewBag.Subjects = new SelectList(new List<string>
            {
                "Research Methodology (MCA21)",
                "AI & ML Lab (MCA122)",
                "Networking with Linux Lab (MCA127)",
                "Artificial Intelligence (MCA22)",
                "DevOps Lab (MCA123)",
                "Computer Networks (MCABR4)",
                "Mini Project (MCAP21)",
                "Robotic Process Automation (MCAE242)",
                "Soft Skill Development (MCAL21)",
                "Research Methodology Tutorial (MCA21)",
                "Information Security (MCA23)",
                "Robotic Process Automation Lab (MCALE242)",
                "User Interface Lab (MCAL26)",
                "Management Information System (MCALE252)",
                "Advanced Web Technologies Lab (MCAL25)",
                "Discrete Mathematics (MCABRS)",
                "Operating System (MCABR)"
            });

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttendance(Attendance model)
        {
            if (ModelState.IsValid)
            {
                // Ensure RollNo is set from StudentId
                if (int.TryParse(model.StudentId, out int rollNo))
                {
                    model.RollNo = rollNo;
                }

                _context.Attendance.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageAttendance");
            }

            // If we got this far, something failed, redisplay form
            // Repopulate dropdowns
            var students = await _userManager.GetUsersInRoleAsync("Student");
            ViewBag.Students = new SelectList(students
                .Where(s => !string.IsNullOrEmpty(s.StudentId))
                .Select(s => new 
                {
                    StudentId = s.StudentId,
                    DisplayName = $"{s.FullName} ({s.StudentId})"
                }), "StudentId", "DisplayName", model.StudentId);

            ViewBag.Subjects = new SelectList(new List<string>
            {
                "Research Methodology (MCA21)",
                "AI & ML Lab (MCA122)",
                "Networking with Linux Lab (MCA127)",
                "Artificial Intelligence (MCA22)",
                "DevOps Lab (MCA123)",
                "Computer Networks (MCABR4)",
                "Mini Project (MCAP21)",
                "Robotic Process Automation (MCAE242)",
                "Soft Skill Development (MCAL21)",
                "Research Methodology Tutorial (MCA21)",
                "Information Security (MCA23)",
                "Robotic Process Automation Lab (MCALE242)",
                "User Interface Lab (MCAL26)",
                "Management Information System (MCALE252)",
                "Advanced Web Technologies Lab (MCAL25)",
                "Discrete Mathematics (MCABRS)",
                "Operating System (MCABR)"
            }, model.Subject);

            return View(model);
        }

        public async Task<IActionResult> EditAttendance(int id)
        {
            var attendance = await _context.Attendance.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            // Get student name for display
            var student = await _userManager.Users
                .FirstOrDefaultAsync(u => u.StudentId == attendance.StudentId);
            ViewBag.StudentName = student != null ? $"{student.FullName} ({student.StudentId})" : "Unknown Student";

            // Populate subjects dropdown
            ViewBag.Subjects = new SelectList(new List<string>
            {
                "Research Methodology (MCA21)",
                "AI & ML Lab (MCA122)",
                "Networking with Linux Lab (MCA127)",
                "Artificial Intelligence (MCA22)",
                "DevOps Lab (MCA123)",
                "Computer Networks (MCABR4)",
                "Mini Project (MCAP21)",
                "Robotic Process Automation (MCAE242)",
                "Soft Skill Development (MCAL21)",
                "Research Methodology Tutorial (MCA21)",
                "Information Security (MCA23)",
                "Robotic Process Automation Lab (MCALE242)",
                "User Interface Lab (MCAL26)",
                "Management Information System (MCALE252)",
                "Advanced Web Technologies Lab (MCAL25)",
                "Discrete Mathematics (MCABRS)",
                "Operating System (MCABR)"
            }, attendance.Subject);

            return View(attendance);
        }

        [HttpPost]
        public async Task<IActionResult> EditAttendance(Attendance model)
        {
            if (ModelState.IsValid)
            {
                _context.Attendance.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageAttendance");
            }

            // If we got this far, something failed, redisplay form
            var student = await _userManager.Users
                .FirstOrDefaultAsync(u => u.StudentId == model.StudentId);
            ViewBag.StudentName = student != null ? $"{student.FullName} ({student.StudentId})" : "Unknown Student";

            ViewBag.Subjects = new SelectList(new List<string>
            {
                "Research Methodology (MCA21)",
                "AI & ML Lab (MCA122)",
                "Networking with Linux Lab (MCA127)",
                "Artificial Intelligence (MCA22)",
                "DevOps Lab (MCA123)",
                "Computer Networks (MCABR4)",
                "Mini Project (MCAP21)",
                "Robotic Process Automation (MCAE242)",
                "Soft Skill Development (MCAL21)",
                "Research Methodology Tutorial (MCA21)",
                "Information Security (MCA23)",
                "Robotic Process Automation Lab (MCALE242)",
                "User Interface Lab (MCAL26)",
                "Management Information System (MCALE252)",
                "Advanced Web Technologies Lab (MCAL25)",
                "Discrete Mathematics (MCABRS)",
                "Operating System (MCABR)"
            }, model.Subject);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var attendance = await _context.Attendance.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendance.Remove(attendance);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageAttendance");
        }

        #endregion

        #region Marks Management

        public async Task<IActionResult> ManageMarks()
        {
            var users = await _userManager.Users.ToListAsync();
            var marksViewModel = new List<MarksManagementViewModel>();

            foreach (var user in users)
            {
                if(!int.TryParse(user.StudentId, out int rollNo))
                {
                    continue;
                }

                var marks = await _context.Marks
                    .Where(m => m.RollNo == rollNo)
                    .OrderByDescending(m => m.ExamDate)
                    .ToListAsync();

                marksViewModel.Add(new MarksManagementViewModel
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    RollNo = rollNo,
                    Course = user.Course,
                    MarksRecords = marks
                });
            }

            return View(marksViewModel);
        }

        public async Task<IActionResult> CreateMarks()
        {
            // Populate students dropdown
            var students = await _userManager.GetUsersInRoleAsync("Student");
            ViewBag.Students = new SelectList(students
                .Where(s => !string.IsNullOrEmpty(s.StudentId))
                .Select(s => new 
                {
                    StudentId = s.StudentId,
                    DisplayName = $"{s.FullName} ({s.StudentId})"
                }), "StudentId", "DisplayName");

            // Populate subjects dropdown
            ViewBag.Subjects = new SelectList(new List<string>
            {
                "Research Methodology (MCA21)",
                "AI & ML Lab (MCA122)",
                "Networking with Linux Lab (MCA127)",
                "Artificial Intelligence (MCA22)",
                "DevOps Lab (MCA123)",
                "Computer Networks (MCABR4)",
                "Mini Project (MCAP21)",
                "Robotic Process Automation (MCAE242)",
                "Soft Skill Development (MCAL21)",
                "Research Methodology Tutorial (MCA21)",
                "Information Security (MCA23)",
                "Robotic Process Automation Lab (MCALE242)",
                "User Interface Lab (MCAL26)",
                "Management Information System (MCALE252)",
                "Advanced Web Technologies Lab (MCAL25)",
                "Discrete Mathematics (MCABRS)",
                "Operating System (MCABR)"
            });

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMarks(Marks model)
        {
            if (ModelState.IsValid)
            {
                // Ensure RollNo is set from StudentId
                if (int.TryParse(model.StudentId, out int rollNo))
                {
                    model.RollNo = rollNo;
                }

                _context.Marks.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageMarks");
            }

            // If we got this far, something failed, redisplay form
            // Repopulate dropdowns
            var students = await _userManager.GetUsersInRoleAsync("Student");
            ViewBag.Students = new SelectList(students
                .Where(s => !string.IsNullOrEmpty(s.StudentId))
                .Select(s => new 
                {
                    StudentId = s.StudentId,
                    DisplayName = $"{s.FullName} ({s.StudentId})"
                }), "StudentId", "DisplayName", model.StudentId);

            ViewBag.Subjects = new SelectList(new List<string>
            {
                "Research Methodology (MCA21)",
                "AI & ML Lab (MCA122)",
                "Networking with Linux Lab (MCA127)",
                "Artificial Intelligence (MCA22)",
                "DevOps Lab (MCA123)",
                "Computer Networks (MCABR4)",
                "Mini Project (MCAP21)",
                "Robotic Process Automation (MCAE242)",
                "Soft Skill Development (MCAL21)",
                "Research Methodology Tutorial (MCA21)",
                "Information Security (MCA23)",
                "Robotic Process Automation Lab (MCALE242)",
                "User Interface Lab (MCAL26)",
                "Management Information System (MCALE252)",
                "Advanced Web Technologies Lab (MCAL25)",
                "Discrete Mathematics (MCABRS)",
                "Operating System (MCABR)"
            }, model.Subject);

            return View(model);
        }

        public async Task<IActionResult> EditMarks(int id)
        {
            var marks = await _context.Marks.FindAsync(id);
            if (marks == null)
            {
                return NotFound();
            }

            // Get student name for display
            var student = await _userManager.Users
                .FirstOrDefaultAsync(u => u.StudentId == marks.StudentId);
            ViewBag.StudentName = student != null ? $"{student.FullName} ({student.StudentId})" : "Unknown Student";

            // Populate subjects dropdown
            ViewBag.Subjects = new SelectList(new List<string>
            {
                "Research Methodology (MCA21)",
                "AI & ML Lab (MCA122)",
                "Networking with Linux Lab (MCA127)",
                "Artificial Intelligence (MCA22)",
                "DevOps Lab (MCA123)",
                "Computer Networks (MCABR4)",
                "Mini Project (MCAP21)",
                "Robotic Process Automation (MCAE242)",
                "Soft Skill Development (MCAL21)",
                "Research Methodology Tutorial (MCA21)",
                "Information Security (MCA23)",
                "Robotic Process Automation Lab (MCALE242)",
                "User Interface Lab (MCAL26)",
                "Management Information System (MCALE252)",
                "Advanced Web Technologies Lab (MCAL25)",
                "Discrete Mathematics (MCABRS)",
                "Operating System (MCABR)"
            }, marks.Subject);

            return View(marks);
        }

        [HttpPost]
        public async Task<IActionResult> EditMarks(Marks model)
        {
            if (ModelState.IsValid)
            {
                _context.Marks.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageMarks");
            }

            // If we got this far, something failed, redisplay form
            var student = await _userManager.Users
                .FirstOrDefaultAsync(u => u.StudentId == model.StudentId);
            ViewBag.StudentName = student != null ? $"{student.FullName} ({student.StudentId})" : "Unknown Student";

            ViewBag.Subjects = new SelectList(new List<string>
            {
                "Research Methodology (MCA21)",
                "AI & ML Lab (MCA122)",
                "Networking with Linux Lab (MCA127)",
                "Artificial Intelligence (MCA22)",
                "DevOps Lab (MCA123)",
                "Computer Networks (MCABR4)",
                "Mini Project (MCAP21)",
                "Robotic Process Automation (MCAE242)",
                "Soft Skill Development (MCAL21)",
                "Research Methodology Tutorial (MCA21)",
                "Information Security (MCA23)",
                "Robotic Process Automation Lab (MCALE242)",
                "User Interface Lab (MCAL26)",
                "Management Information System (MCALE252)",
                "Advanced Web Technologies Lab (MCAL25)",
                "Discrete Mathematics (MCABRS)",
                "Operating System (MCABR)"
            }, model.Subject);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMarks(int id)
        {
            var marks = await _context.Marks.FindAsync(id);
            if (marks != null)
            {
                _context.Marks.Remove(marks);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageMarks");
        }

        #endregion
    }
} 