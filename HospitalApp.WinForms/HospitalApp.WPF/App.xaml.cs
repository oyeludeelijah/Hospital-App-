using HospitalApp.Data;
using HospitalApp.WPF.Services;
using HospitalApp.WPF.ViewModels;
using HospitalApp.WPF.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace HospitalApp.WPF
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=HospitalAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

            // Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IDialogService, DialogService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<PatientListViewModel>();
            services.AddTransient<PatientDetailsViewModel>();
            services.AddTransient<AppointmentListViewModel>();
            services.AddTransient<AppointmentDetailsViewModel>();
            services.AddTransient<DoctorListViewModel>();
            services.AddTransient<DoctorDetailsViewModel>();
            services.AddTransient<MedicalRecordListViewModel>();
            services.AddTransient<MedicalRecordDetailsViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
            services.AddTransient<LoginView>();
            services.AddTransient<DashboardView>();
            services.AddTransient<PatientListView>();
            services.AddTransient<PatientDetailsView>();
            services.AddTransient<AppointmentListView>();
            services.AddTransient<AppointmentDetailsView>();
            services.AddTransient<DoctorListView>();
            services.AddTransient<DoctorDetailsView>();
            services.AddTransient<MedicalRecordListView>();
            services.AddTransient<MedicalRecordDetailsView>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync();
            }

            base.OnExit(e);
        }
    }
}
