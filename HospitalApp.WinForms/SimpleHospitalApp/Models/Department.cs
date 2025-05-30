using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleHospitalApp.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
