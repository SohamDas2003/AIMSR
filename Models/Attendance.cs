using System;
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
        public string StudentId { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Absent"; // Default value to avoid null

        public string? Notes { get; set; }
    }
}
