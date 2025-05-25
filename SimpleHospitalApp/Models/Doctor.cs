using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleHospitalApp.Models
{
    public class Doctor
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
        [StringLength(50)]
        public string Specialization { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string ContactNumber { get; set; } = string.Empty;
        
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string FullName => $"Dr. {FirstName} {LastName}";
        
        // Foreign keys
        public int? DepartmentId { get; set; }
        
        // Navigation properties
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}