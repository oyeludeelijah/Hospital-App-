using System;

namespace HospitalApp.Web.HospitalApi.Models
{
    public class SimpleAppointment
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Scheduled, Completed, Cancelled
    }
}
