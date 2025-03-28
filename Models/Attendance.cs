using System.ComponentModel.DataAnnotations;

namespace AIMSR.Models
{
    public class Attendance
    {
        [Key] // Ensure primary key exists
        public int Id { get; set; }

        [Required] // RollNo must not be null
        public int RollNo { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string? Subject { get; set; } // Nullable to avoid CS8618 error

        [Required]
        public string Status { get; set; } = "Absent"; // Default value to avoid null
    }
}
