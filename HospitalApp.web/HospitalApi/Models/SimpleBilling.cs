using System;

namespace HospitalApp.Web.HospitalApi.Models
{
    public class SimpleBilling
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime BillingDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty; // Paid, Pending, Overdue
        public string PaymentMethod { get; set; } = string.Empty; // Cash, Credit Card, Insurance, etc.
    }
}
