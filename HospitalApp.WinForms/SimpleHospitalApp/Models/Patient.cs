using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleHospitalApp.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string ContactNumber { get; set; } = string.Empty;
        
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        
        public string MedicalHistory { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";
        public int Age => DateTime.Now.Year - DateOfBirth.Year - (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);

        // Navigation properties
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}
