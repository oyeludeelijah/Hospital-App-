using System;

namespace HospitalApp.Web.HospitalApi.Models
{
    public enum AppointmentStatus
    {
        Scheduled,
        Confirmed,
        Completed,
        Cancelled,
        NoShow
    }

    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public MedicalRecord? MedicalRecord { get; set; }
        
        public bool IsUpcoming => 
            (AppointmentDate.Date > DateTime.Now.Date) || 
            (AppointmentDate.Date == DateTime.Now.Date && StartTime > DateTime.Now.TimeOfDay);
    }
}
