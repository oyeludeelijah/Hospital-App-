using HospitalApp.Core.Models;
using HospitalApp.Data;
using HospitalApp.WPF.Commands;
using HospitalApp.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace HospitalApp.WPF.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INavigationService _navigationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IDialogService _dialogService;

        public string WelcomeMessage => $"Welcome, {_authenticationService.CurrentUser?.FullName ?? "User"}";
        
        private int _totalPatients;
        public int TotalPatients
        {
            get => _totalPatients;
            set => SetProperty(ref _totalPatients, value);
        }
        
        private int _todayAppointments;
        public int TodayAppointments
        {
            get => _todayAppointments;
            set => SetProperty(ref _todayAppointments, value);
        }
        
        private int _admittedPatients;
        public int AdmittedPatients
        {
            get => _admittedPatients;
            set => SetProperty(ref _admittedPatients, value);
        }
        
        private int _availableDoctors;
        public int AvailableDoctors
        {
            get => _availableDoctors;
            set => SetProperty(ref _availableDoctors, value);
        }

        public ObservableCollection<AppointmentViewModel> TodayAppointmentsList { get; } = new ObservableCollection<AppointmentViewModel>();
        public ObservableCollection<ActivityViewModel> RecentActivities { get; } = new ObservableCollection<ActivityViewModel>();

        public ICommand QuickActionCommand { get; }
        public ICommand ViewAllAppointmentsCommand { get; }

        public DashboardViewModel(
            IUnitOfWork unitOfWork,
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            IDialogService dialogService)
        {
            _unitOfWork = unitOfWork;
            _navigationService = navigationService;
            _authenticationService = authenticationService;
            _dialogService = dialogService;

            QuickActionCommand = new RelayCommand<string>(ExecuteQuickAction);
            ViewAllAppointmentsCommand = new RelayCommand<object>(_ => _navigationService.NavigateTo<AppointmentListViewModel>());

            LoadDashboardDataAsync();
        }

        private async void LoadDashboardDataAsync()
        {
            try
            {
                IsBusy = true;

                // Load statistics
                TotalPatients = await _unitOfWork.Patients.CountAsync(p => true);
                TodayAppointments = await _unitOfWork.Appointments.CountAsync(a => 
                    a.AppointmentDate.Date == DateTime.Today && 
                    a.Status != AppointmentStatus.Cancelled);
                AdmittedPatients = await _unitOfWork.Admissions.CountAsync(a => 
                    a.Status == AdmissionStatus.Admitted);
                AvailableDoctors = await _unitOfWork.Doctors.CountAsync(d => 
                    d.IsAvailable);

                // Load today's appointments
                var appointments = await _unitOfWork.Appointments.FindAsync(a => 
                    a.AppointmentDate.Date == DateTime.Today && 
                    a.Status != AppointmentStatus.Cancelled);

                TodayAppointmentsList.Clear();
                foreach (var appointment in appointments)
                {
                    TodayAppointmentsList.Add(new AppointmentViewModel
                    {
                        Time = $"{appointment.StartTime:hh\\:mm tt}",
                        PatientName = appointment.Patient.User.FullName,
                        Purpose = appointment.Purpose,
                        Status = appointment.Status.ToString(),
                        StatusColor = GetStatusColor(appointment.Status)
                    });
                }

                // Load recent activities (this would typically come from an activity log)
                // For demonstration, we'll add some sample activities
                RecentActivities.Clear();
                RecentActivities.Add(new ActivityViewModel
                {
                    Description = "New patient registered: John Smith",
                    Time = "Today, 10:30 AM"
                });
                RecentActivities.Add(new ActivityViewModel
                {
                    Description = "Dr. Sarah Johnson completed appointment with patient #12345",
                    Time = "Today, 09:45 AM"
                });
                RecentActivities.Add(new ActivityViewModel
                {
                    Description = "New lab results uploaded for patient #54321",
                    Time = "Today, 08:15 AM"
                });
                RecentActivities.Add(new ActivityViewModel
                {
                    Description = "Patient #67890 discharged from Room 302",
                    Time = "Yesterday, 05:20 PM"
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading dashboard data: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ExecuteQuickAction(string? action)
        {
            if (string.IsNullOrEmpty(action)) return;

            switch (action)
            {
                case "NewPatient":
                    _navigationService.NavigateTo<PatientDetailsViewModel>();
                    break;
                case "NewAppointment":
                    _navigationService.NavigateTo<AppointmentDetailsViewModel>();
                    break;
                case "SearchPatient":
                    _navigationService.NavigateTo<PatientListViewModel>();
                    break;
                case "Reports":
                    _dialogService.ShowMessage("Reports functionality is coming soon!", "Coming Soon");
                    break;
                default:
                    _dialogService.ShowWarning($"Action '{action}' is not implemented yet.");
                    break;
            }
        }

        private Brush GetStatusColor(AppointmentStatus status)
        {
            return status switch
            {
                AppointmentStatus.Scheduled => new SolidColorBrush(Colors.Orange),
                AppointmentStatus.Confirmed => new SolidColorBrush(Colors.Blue),
                AppointmentStatus.Completed => new SolidColorBrush(Colors.Green),
                AppointmentStatus.Cancelled => new SolidColorBrush(Colors.Red),
                AppointmentStatus.NoShow => new SolidColorBrush(Colors.Red),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
    }

    public class AppointmentViewModel
    {
        public string Time { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Brush StatusColor { get; set; } = Brushes.Gray;
    }

    public class ActivityViewModel
    {
        public string Description { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
    }
}
