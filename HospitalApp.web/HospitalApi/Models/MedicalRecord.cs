using System;
using System.Collections.Generic;

namespace HospitalApp.Web.HospitalApi.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public int? NurseId { get; set; }
        public Nurse? Nurse { get; set; }
        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }
        public DateTime RecordDate { get; set; } = DateTime.Now;
        public string Diagnosis { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool IsConfidential { get; set; } = false;
        
        // Navigation properties
        public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public List<LabTest> LabTests { get; set; } = new List<LabTest>();
    }
}
