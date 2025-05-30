using System;

namespace HospitalApp.Web.HospitalApi.Models
{
    public enum BillStatus
    {
        Pending,
        PartiallyPaid,
        Paid,
        Overdue,
        Cancelled
    }

    public class Bill
    {
        public int Id { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public DateTime BillDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public BillStatus Status { get; set; } = BillStatus.Pending;
        public string Notes { get; set; } = string.Empty;
        
        public decimal RemainingAmount => TotalAmount - PaidAmount;
    }
}
