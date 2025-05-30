using System;
using System.Collections.Generic;

namespace HospitalApp.Web.HospitalApi.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string LicenseNumber { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; } = DateTime.Now;
        public bool IsAvailable { get; set; } = true;
        
        // Navigation properties
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public List<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public List<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
    }
}
