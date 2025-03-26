using System.ComponentModel.DataAnnotations;

namespace AIMSR.Models
{
    public class Profile
    {
        [Key]
        public int RollNo { get; set; } // Primary Key, acting as Roll No.

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Course { get; set; } // Course Name (e.g., MCA, BCA, etc.)

        [Required]
        public int Year { get; set; } // Academic Year (e.g., 1, 2, 3)
    }
}
