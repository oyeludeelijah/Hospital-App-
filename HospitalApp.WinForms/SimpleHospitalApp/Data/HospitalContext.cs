using Microsoft.EntityFrameworkCore;
using SimpleHospitalApp.Models;
using System;
using System.IO;

namespace SimpleHospitalApp.Data
{
    public class HospitalContext : DbContext
    {
        // DbSets for our tables
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hospital.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships and seed data
            base.OnModelCreating(modelBuilder);

            // Seed user data with default admin account
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "admin", // In a real app, use password hashing
                    Email = "admin@hospital.com",
                    Role = "Admin",
                    CreatedDate = DateTime.Now
                }
            );
            
            // Seed sample patients
            modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    Gender = "Male",
                    ContactNumber = "123-456-7890",
                    Email = "john.doe@example.com",
                    Address = "123 Main St",
                    MedicalHistory = "No significant history"
                },
                new Patient
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Gender = "Female",
                    ContactNumber = "987-654-3210",
                    Email = "jane.smith@example.com",
                    Address = "456 Oak Ave",
                    MedicalHistory = "Allergic to penicillin"
                }
            );

            // Seed department data
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Cardiology", Description = "Heart-related treatments" },
                new Department { Id = 2, Name = "Pediatrics", Description = "Child healthcare" },
                new Department { Id = 3, Name = "Orthopedics", Description = "Bone and joint care" }
            );
        }
    }
}
