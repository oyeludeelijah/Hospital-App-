using System;
using System.Collections.Generic;

namespace HospitalApp.Core.Models
{
    public enum BillStatus
    {
        Pending,
        PartiallyPaid,
        Paid,
        Overdue,
        Cancelled
    }

    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        DebitCard,
        Insurance,
        BankTransfer,
        Other
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
        public decimal? InsuranceCoverage { get; set; }
        public string? InsuranceDetails { get; set; }
        public BillStatus Status { get; set; } = BillStatus.Pending;
        public string Notes { get; set; } = string.Empty;
        
        // Navigation properties
        public List<BillItem> BillItems { get; set; } = new List<BillItem>();
        public List<Payment> Payments { get; set; } = new List<Payment>();
        
        public decimal RemainingAmount => TotalAmount - PaidAmount - (InsuranceCoverage ?? 0);
        public bool IsFullyPaid => PaidAmount + (InsuranceCoverage ?? 0) >= TotalAmount;
    }

    public class BillItem
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public Bill Bill { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public string ServiceCode { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice => (UnitPrice * Quantity) - Discount;
    }

    public class Payment
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public Bill Bill { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
