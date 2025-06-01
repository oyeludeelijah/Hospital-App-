using Microsoft.EntityFrameworkCore;
using HospitalApp.Web.HospitalApi.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HospitalApp.Web.HospitalApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            // Disable change tracking for better performance in production
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<SimpleDoctor> SimpleDoctors { get; set; } = null!;
        public DbSet<SimpleAppointment> SimpleAppointments { get; set; } = null!;
        public DbSet<SimpleBilling> SimpleBillings { get; set; } = null!;
        public DbSet<SimpleDepartment> SimpleDepartments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and constraints
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                // Explicitly define the one-to-one relationship with Patient
                entity.HasOne(u => u.Patient)
                    .WithOne(p => p.User)
                    .HasForeignKey<Patient>(p => p.UserId);

                // Explicitly define the one-to-one relationship with Doctor
                entity.HasOne(u => u.Doctor)
                    .WithOne(d => d.User)
                    .HasForeignKey<Doctor>(d => d.UserId);
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasIndex(e => e.LicenseNumber).IsUnique();
                // This configuration is now handled by the User entity's configuration
                // entity.HasOne(d => d.User)
                //    .WithOne()
                //    .HasForeignKey<Doctor>(d => d.UserId)
                //    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasIndex(e => e.PatientNumber).IsUnique();
                // This configuration is now handled by the User entity's configuration
                // entity.HasOne(p => p.User)
                //    .WithOne()
                //    .HasForeignKey<Patient>(p => p.UserId)
                //    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.ConfigureWarnings(warnings =>
                {
                    warnings.Log(RelationalEventId.ConnectionOpened, RelationalEventId.ConnectionClosed);
                    warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                    warnings.Ignore(RelationalEventId.PendingModelChangesWarning);
                });
            }
        }
    }
}
