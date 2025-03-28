using Microsoft.AspNetCore.Identity;

namespace AIMSR.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string StudentId { get; set; }
        public string Course { get; set; }
        public string Department { get; set; }
    }
} 