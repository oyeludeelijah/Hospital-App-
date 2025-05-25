using SimpleHospitalApp.Forms;
using SimpleHospitalApp.Models;
using SimpleHospitalApp.Data;
using System.Linq;

namespace SimpleHospitalApp;

static class Program
{
    // Current logged-in user
    public static User? CurrentUser { get; set; }

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        
        // Initialize database
        InitializeDatabase();
        
        // Start with the login form
        Application.Run(new LoginForm());
    }
    
    static void InitializeDatabase()
    {
        try
        {
            using (var context = new HospitalContext())
            {
                // Ensure database is created
                context.Database.EnsureCreated();
                
                // Check if we need to seed initial admin user
                if (!context.Users.Any())
                {
                    var adminUser = new User
                    {
                        Username = "admin",
                        PasswordHash = "admin", // In a real app, use password hashing
                        Email = "admin@hospital.com",
                        Role = "Admin",
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };
                    
                    context.Users.Add(adminUser);
                    context.SaveChanges();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Database initialization error: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}