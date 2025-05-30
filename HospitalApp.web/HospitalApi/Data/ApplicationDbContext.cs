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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add specific model configurations here if needed
        }
    }
}
