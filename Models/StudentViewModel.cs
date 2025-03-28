namespace AIMSR.Models
{
    public class StudentViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string StudentId { get; set; }
        public string Course { get; set; }
        public string Department { get; set; }
        
        // Additional student information can be added here
        public int? Semester { get; set; }
        public string Section { get; set; }
        public string EnrollmentYear { get; set; }
    }
} 