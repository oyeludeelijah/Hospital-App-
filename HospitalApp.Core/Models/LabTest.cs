using System;

namespace HospitalApp.Core.Models
{
    public enum LabTestStatus
    {
        Requested,
        SampleCollected,
        InProgress,
        Completed,
        Cancelled
    }

    public class LabTest
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public int RequestedByDoctorId { get; set; }
        public Doctor RequestedByDoctor { get; set; } = null!;
        public int? MedicalRecordId { get; set; }
        public MedicalRecord? MedicalRecord { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string TestCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateTime? SampleCollectionDate { get; set; }
        public DateTime? ResultDate { get; set; }
        public string? Results { get; set; }
        public string? ReferenceRange { get; set; }
        public string? Interpretation { get; set; }
        public LabTestStatus Status { get; set; } = LabTestStatus.Requested;
        public string Notes { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        
        public bool HasResults => ResultDate != null && !string.IsNullOrEmpty(Results);
    }
}
