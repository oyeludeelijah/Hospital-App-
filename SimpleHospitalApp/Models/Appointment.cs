using System;

namespace SimpleHospitalApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Canceled
        public string Notes { get; set; } = string.Empty;

        // References to related objects (for in-memory relationships)
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
