using System;
using System.Collections.Generic;

namespace HospitalApp.Core.Models
{
    public enum UserRole
    {
        Admin,
        Doctor,
        Nurse,
        Patient
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public Doctor? Doctor { get; set; }
        public Nurse? Nurse { get; set; }
        public Patient? Patient { get; set; }
        
        public string FullName => $"{FirstName} {LastName}";
    }
}
