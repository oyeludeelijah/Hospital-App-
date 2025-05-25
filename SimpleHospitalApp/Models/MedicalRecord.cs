using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleHospitalApp.Models
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int PatientId { get; set; }
        
        public int? AppointmentId { get; set; }
        
        [Required]
        public DateTime RecordDate { get; set; } = DateTime.Now;
        
        [Required]
        [StringLength(200)]
        public string Diagnosis { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Treatment { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Prescription { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;
        
        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
        
        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }
    }
}
