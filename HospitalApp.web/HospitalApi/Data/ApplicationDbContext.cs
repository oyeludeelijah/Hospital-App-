using Microsoft.EntityFrameworkCore;
using HospitalApp.Web.HospitalApi.Models;

namespace HospitalApp.Web.HospitalApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
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
            // Add specific model configurations here if needed
        }
    }
}
