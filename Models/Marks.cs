using System;
using System.ComponentModel.DataAnnotations;

namespace AIMSR.Models
{
    public class Marks
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RollNo { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string ExamType { get; set; } = string.Empty; // "Midterm", "Final", "Assignment"

        [Required]
        [Range(0, 100)]
        public decimal ObtainedMarks { get; set; }

        [Required]
        [Range(1, 100)]
        public decimal MaxMarks { get; set; }

        [Required]
        public DateTime ExamDate { get; set; } = DateTime.Now;

        [Required]
        public string Semester { get; set; } = string.Empty;

        [Required]
        public string AcademicYear { get; set; } = string.Empty;

        public string? Remarks { get; set; }
    }
} 