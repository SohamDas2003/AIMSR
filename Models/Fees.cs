using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIMSR.Models
{
    public class Fees
    {
        [Key]
        public int Id { get; set; } // Primary Key (Auto-Increment)

        [Required]
        public int RollNo { get; set; } // Foreign Key (Profile Roll No)

        [Required]
        public decimal TotalFees { get; set; } // Total Fees Amount

        [Required]
        public decimal AmountPaid { get; set; } // Amount Paid by Student

        [Required]
        public decimal DueAmount { get; set; } // Remaining Fees

        [Required]
        public DateTime PaymentDate { get; set; } // Last Payment Date

        [Required]
        public string PaymentStatus { get; set; } // "Paid" or "Pending"

        [ForeignKey("RollNo")]
        public Profile Profile { get; set; } // Navigation Property
    }
}
