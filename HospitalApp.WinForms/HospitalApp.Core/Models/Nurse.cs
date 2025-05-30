using System;
using System.Collections.Generic;

namespace HospitalApp.Core.Models
{
    public class Nurse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string LicenseNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; } = DateTime.Now;
        
        // Navigation properties
        public List<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}
