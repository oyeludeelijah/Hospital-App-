using System;
using System.Collections.Generic;

namespace HospitalApp.Web.HospitalApi.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public int? MedicalRecordId { get; set; }
        public MedicalRecord? MedicalRecord { get; set; }
        public DateTime PrescriptionDate { get; set; } = DateTime.Now;
        public DateTime? ExpiryDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        
        // Navigation properties
        public List<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
        
        public bool IsActive => ExpiryDate == null || ExpiryDate > DateTime.Now;
    }

    public class PrescriptionItem
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; } = null!;
        public int MedicationId { get; set; }
        public Medication Medication { get; set; } = null!;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public int Duration { get; set; } // in days
        public string Instructions { get; set; } = string.Empty;
    }

    public class Medication
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string DosageForm { get; set; } = string.Empty; // tablet, capsule, liquid, etc.
        public string Strength { get; set; } = string.Empty;
        public bool RequiresPrescription { get; set; } = true;
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        
        // Navigation properties
        public List<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
    }
}
