using System;
using System.Collections.Generic;

namespace HospitalApp.Core.Models
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public class Patient
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string PatientNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public string BloodType { get; set; } = string.Empty;
        public bool HasInsurance { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? InsuranceNumber { get; set; }
        public DateTime RegisteredDate { get; set; } = DateTime.Now;
        
        // Navigation properties
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public List<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public List<Admission> Admissions { get; set; } = new List<Admission>();
        public List<Bill> Bills { get; set; } = new List<Bill>();
        
        public int Age => DateTime.Now.Year - DateOfBirth.Year - 
                         (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
    }
}
