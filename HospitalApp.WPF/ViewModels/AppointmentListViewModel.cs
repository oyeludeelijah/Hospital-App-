using HospitalApp.Core.Models;
using HospitalApp.Data;
using HospitalApp.WPF.Commands;
using HospitalApp.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace HospitalApp.WPF.ViewModels
{
    public class AppointmentListViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IAuthenticationService _authenticationService;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    LoadAppointmentsAsync();
                }
            }
        }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    LoadAppointmentsAsync();
                }
            }
        }

        private AppointmentViewModel? _selectedAppointment;
        public AppointmentViewModel? SelectedAppointment
        {
            get => _selectedAppointment;
            set => SetProperty(ref _selectedAppointment, value);
        }

        public ObservableCollection<AppointmentViewModel> Appointments { get; } = new ObservableCollection<AppointmentViewModel>();

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    LoadAppointmentsAsync();
                    OnPropertyChanged(nameof(PaginationInfo));
                    OnPropertyChanged(nameof(CanGoToPreviousPage));
                    OnPropertyChanged(nameof(CanGoToNextPage));
                }
            }
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (SetProperty(ref _pageSize, value))
                {
                    LoadAppointmentsAsync();
                    OnPropertyChanged(nameof(PaginationInfo));
                }
            }
        }

        private int _totalAppointments;
        public int TotalAppointments
        {
            get => _totalAppointments;
            set
            {
                if (SetProperty(ref _totalAppointments, value))
                {
                    OnPropertyChanged(nameof(PaginationInfo));
                    OnPropertyChanged(nameof(CanGoToNextPage));
                }
            }
        }

        public string PaginationInfo => $"Page {CurrentPage} of {Math.Max(1, (int)Math.Ceiling((double)TotalAppointments / PageSize))}";
        public bool CanGoToPreviousPage => CurrentPage > 1;
        public bool CanGoToNextPage => CurrentPage < Math.Ceiling((double)TotalAppointments / PageSize);
        public bool HasNoResults => !IsBusy && Appointments.Count == 0;

        public ICommand AddNewAppointmentCommand { get; }
        public ICommand ViewAppointmentCommand { get; }
        public ICommand EditAppointmentCommand { get; }
        public ICommand CancelAppointmentCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public AppointmentListViewModel(
            IUnitOfWork unitOfWork,
            INavigationService navigationService,
            IDialogService dialogService,
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _authenticationService = authenticationService;

            AddNewAppointmentCommand = new RelayCommand<object>(_ => NavigateToAppointmentDetails());
            ViewAppointmentCommand = new RelayCommand<AppointmentViewModel>(ViewAppointment);
            EditAppointmentCommand = new RelayCommand<AppointmentViewModel>(EditAppointment);
            CancelAppointmentCommand = new RelayCommand<AppointmentViewModel>(CancelAppointment);
            FilterCommand = new RelayCommand<object>(_ => LoadAppointmentsAsync());
            PreviousPageCommand = new RelayCommand<object>(_ => CurrentPage--, _ => CanGoToPreviousPage);
            NextPageCommand = new RelayCommand<object>(_ => CurrentPage++, _ => CanGoToNextPage);

            LoadAppointmentsAsync();
        }

        private async void LoadAppointmentsAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                // Calculate skip for pagination
                int skip = (CurrentPage - 1) * PageSize;

                // Build the filter predicate
                Func<Appointment, bool> predicate = a =>
                    (string.IsNullOrEmpty(SearchText) ||
                     a.Patient.User.FirstName.Contains(SearchText) ||
                     a.Patient.User.LastName.Contains(SearchText) ||
                     a.Doctor.User.FirstName.Contains(SearchText) ||
                     a.Doctor.User.LastName.Contains(SearchText) ||
                     a.Purpose.Contains(SearchText)) &&
                    (!SelectedDate.HasValue || a.AppointmentDate.Date == SelectedDate.Value.Date);

                // Get total count for pagination
                TotalAppointments = await _unitOfWork.Appointments.CountAsync(predicate);

                // Get appointments for current page
                var appointments = await _unitOfWork.Appointments.FindAsync(predicate);

                // Apply pagination
                appointments = appointments.Skip(skip).Take(PageSize).ToList();

                Appointments.Clear();
                foreach (var appointment in appointments)
                {
                    Appointments.Add(new AppointmentViewModel
                    {
                        Id = appointment.Id,
                        AppointmentDate = appointment.AppointmentDate,
                        TimeSlot = $"{appointment.StartTime:hh\\:mm tt} - {appointment.EndTime:hh\\:mm tt}",
                        PatientId = appointment.PatientId,
                        PatientName = appointment.Patient.User.FullName,
                        DoctorId = appointment.DoctorId,
                        DoctorName = appointment.Doctor.User.FullName,
                        Purpose = appointment.Purpose,
                        Notes = appointment.Notes,
                        Status = appointment.Status.ToString(),
                        StatusColor = GetStatusColor(appointment.Status),
                        IsUpcoming = appointment.IsUpcoming
                    });
                }

                OnPropertyChanged(nameof(HasNoResults));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading appointments: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void NavigateToAppointmentDetails(int? appointmentId = null)
        {
            if (appointmentId.HasValue)
            {
                _navigationService.NavigateTo<AppointmentDetailsViewModel>(appointmentId.Value);
            }
            else
            {
                _navigationService.NavigateTo<AppointmentDetailsViewModel>();
            }
        }

        private void ViewAppointment(AppointmentViewModel? appointment)
        {
            if (appointment == null) return;
            NavigateToAppointmentDetails(appointment.Id);
        }

        private void EditAppointment(AppointmentViewModel? appointment)
        {
            if (appointment == null) return;
            NavigateToAppointmentDetails(appointment.Id);
        }

        private async void CancelAppointment(AppointmentViewModel? appointment)
        {
            if (appointment == null) return;

            bool confirmed = await _dialogService.ShowConfirmationAsync(
                $"Are you sure you want to cancel this appointment with {appointment.PatientName}?",
                "Confirm Cancellation");

            if (!confirmed) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var appointmentEntity = await _unitOfWork.Appointments.GetByIdAsync(appointment.Id);
                if (appointmentEntity == null)
                {
                    _dialogService.ShowError("Appointment not found.");
                    return;
                }

                appointmentEntity.Status = AppointmentStatus.Cancelled;
                appointmentEntity.UpdatedAt = DateTime.Now;

                _unitOfWork.Appointments.Update(appointmentEntity);
                await _unitOfWork.CompleteAsync();

                appointment.Status = AppointmentStatus.Cancelled.ToString();
                appointment.StatusColor = GetStatusColor(AppointmentStatus.Cancelled);

                _dialogService.ShowMessage("Appointment has been cancelled successfully.", "Success");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error cancelling appointment: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
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
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Brush StatusColor { get; set; } = Brushes.Gray;
        public bool IsUpcoming { get; set; }
    }
}
