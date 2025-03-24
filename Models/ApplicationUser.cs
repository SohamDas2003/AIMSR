using Microsoft.AspNetCore.Identity;

namespace AIMSR.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string ProfileImage { get; set; } // Path to profile image
        public string Department { get; set; } // Relevant for teachers
        public string Course { get; set; } // Relevant for students
    }
}
