using System;

namespace HospitalApp.Core.Models
{
    public enum AdmissionStatus
    {
        Admitted,
        Discharged,
        Transferred
    }

    public class Admission
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public int AdmittedByDoctorId { get; set; }
        public Doctor AdmittedByDoctor { get; set; } = null!;
        public int? DischargedByDoctorId { get; set; }
        public Doctor? DischargedByDoctor { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public DateTime AdmissionDate { get; set; } = DateTime.Now;
        public DateTime? DischargeDate { get; set; }
        public string AdmissionReason { get; set; } = string.Empty;
        public string? DischargeNotes { get; set; }
        public string? TreatmentSummary { get; set; }
        public AdmissionStatus Status { get; set; } = AdmissionStatus.Admitted;
        
        public int StayDuration => DischargeDate.HasValue 
            ? (DischargeDate.Value - AdmissionDate).Days 
            : (DateTime.Now - AdmissionDate).Days;
    }

    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty; // Ward, Private, ICU, etc.
        public int Floor { get; set; }
        public string Wing { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal DailyRate { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string Notes { get; set; } = string.Empty;
    }
}
