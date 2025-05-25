using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleHospitalApp.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int DoctorId { get; set; }
        
        [Required]
        public DateTime AppointmentDate { get; set; }
        
        [StringLength(200)]
        public string Purpose { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Canceled
        
        [StringLength(200)]
        public string Notes { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
        
        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }
        
        // Related entities
        public virtual MedicalRecord? MedicalRecord { get; set; }
        public virtual Billing? Billing { get; set; }
    }
}
