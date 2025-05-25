using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleHospitalApp.Models
{
    public class Billing
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int PatientId { get; set; }
        
        public int? AppointmentId { get; set; }
        
        [Required]
        public DateTime BillingDate { get; set; } = DateTime.Now;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Overdue
        
        [StringLength(100)]
        public string PaymentMethod { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
        
        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }
    }
}
