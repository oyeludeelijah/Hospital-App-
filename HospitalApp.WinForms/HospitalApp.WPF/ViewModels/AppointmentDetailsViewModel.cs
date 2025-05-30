using HospitalApp.Core.Models;
using HospitalApp.Data;
using HospitalApp.WPF.Commands;
using HospitalApp.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HospitalApp.WPF.ViewModels
{
    public class AppointmentDetailsViewModel : ViewModelBase, IParameterizedViewModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IAuthenticationService _authenticationService;

        private int? _appointmentId;
        private bool _isEditMode;

        public string PageTitle => _isEditMode ? "Edit Appointment" : "Schedule New Appointment";
        public bool IsEditMode => _isEditMode;

        #region Appointment Properties

        private PatientViewModel? _selectedPatient;
        public PatientViewModel? SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                if (SetProperty(ref _selectedPatient, value))
                {
                    LoadAvailableTimeSlotsAsync();
                }
            }
        }

        private DoctorViewModel? _selectedDoctor;
        public DoctorViewModel? SelectedDoctor
        {
            get => _selectedDoctor;
            set
            {
                if (SetProperty(ref _selectedDoctor, value))
                {
                    LoadAvailableTimeSlotsAsync();
                }
            }
        }

        private DateTime _appointmentDate = DateTime.Today.AddDays(1);
        public DateTime AppointmentDate
        {
            get => _appointmentDate;
            set
            {
                if (SetProperty(ref _appointmentDate, value))
                {
                    LoadAvailableTimeSlotsAsync();
                }
            }
        }

        private string? _selectedTimeSlot;
        public string? SelectedTimeSlot
        {
            get => _selectedTimeSlot;
            set => SetProperty(ref _selectedTimeSlot, value);
        }

        private string _purpose = string.Empty;
        public string Purpose
        {
            get => _purpose;
            set => SetProperty(ref _purpose, value);
        }

        private string _notes = string.Empty;
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        private string _status = AppointmentStatus.Scheduled.ToString();
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public List<string> StatusOptions { get; } = Enum.GetNames(typeof(AppointmentStatus)).ToList();

        #endregion

        public ObservableCollection<PatientViewModel> Patients { get; } = new ObservableCollection<PatientViewModel>();
        public ObservableCollection<DoctorViewModel> Doctors { get; } = new ObservableCollection<DoctorViewModel>();
        public ObservableCollection<string> AvailableTimeSlots { get; } = new ObservableCollection<string>();

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BackToListCommand { get; }
        public ICommand AddNewPatientCommand { get; }

        public AppointmentDetailsViewModel(
            IUnitOfWork unitOfWork,
            INavigationService navigationService,
            IDialogService dialogService,
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _authenticationService = authenticationService;

            SaveCommand = new RelayCommand<object>(_ => SaveAppointmentAsync());
            CancelCommand = new RelayCommand<object>(_ => Cancel());
            BackToListCommand = new RelayCommand<object>(_ => _navigationService.NavigateTo<AppointmentListViewModel>());
            AddNewPatientCommand = new RelayCommand<object>(_ => _navigationService.NavigateTo<PatientDetailsViewModel>());

            LoadPatientsAndDoctorsAsync();
        }

        public void Initialize(object parameter)
        {
            if (parameter is int appointmentId)
            {
                _appointmentId = appointmentId;
                _isEditMode = true;
                LoadAppointmentAsync(appointmentId);
            }
            else
            {
                _appointmentId = null;
                _isEditMode = false;
            }

            OnPropertyChanged(nameof(PageTitle));
            OnPropertyChanged(nameof(IsEditMode));
        }

        private async void LoadPatientsAndDoctorsAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                // Load patients
                var patients = await _unitOfWork.Patients.GetAllAsync();
                Patients.Clear();
                foreach (var patient in patients)
                {
                    Patients.Add(new PatientViewModel
                    {
                        Id = patient.Id,
                        FullName = patient.User.FullName
                    });
                }

                // Load doctors
                var doctors = await _unitOfWork.Doctors.GetAllAsync();
                Doctors.Clear();
                foreach (var doctor in doctors)
                {
                    Doctors.Add(new DoctorViewModel
                    {
                        Id = doctor.Id,
                        FullName = doctor.User.FullName,
                        Specialization = doctor.Specialization
                    });
                }

                // If there are doctors available, select the first one by default
                if (Doctors.Count > 0 && SelectedDoctor == null)
                {
                    SelectedDoctor = Doctors[0];
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void LoadAppointmentAsync(int appointmentId)
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    _dialogService.ShowError("Appointment not found.");
                    _navigationService.NavigateTo<AppointmentListViewModel>();
                    return;
                }

                // Set appointment properties
                AppointmentDate = appointment.AppointmentDate;
                Purpose = appointment.Purpose;
                Notes = appointment.Notes;
                Status = appointment.Status.ToString();

                // Set selected patient
                var patientViewModel = Patients.FirstOrDefault(p => p.Id == appointment.PatientId);
                if (patientViewModel != null)
                {
                    SelectedPatient = patientViewModel;
                }

                // Set selected doctor
                var doctorViewModel = Doctors.FirstOrDefault(d => d.Id == appointment.DoctorId);
                if (doctorViewModel != null)
                {
                    SelectedDoctor = doctorViewModel;
                }

                // Set selected time slot
                SelectedTimeSlot = $"{appointment.StartTime:hh\\:mm tt} - {appointment.EndTime:hh\\:mm tt}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading appointment: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void LoadAvailableTimeSlotsAsync()
        {
            if (SelectedDoctor == null || AppointmentDate == DateTime.MinValue)
            {
                AvailableTimeSlots.Clear();
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                // Get doctor's schedule for the selected date
                var doctorSchedules = await _unitOfWork.DoctorSchedules.FindAsync(ds => 
                    ds.DoctorId == SelectedDoctor.Id && 
                    ds.Day == (WeekDay)AppointmentDate.DayOfWeek && 
                    ds.IsAvailable);

                var doctorSchedule = doctorSchedules.FirstOrDefault();
                if (doctorSchedule == null)
                {
                    AvailableTimeSlots.Clear();
                    ErrorMessage = "The selected doctor is not available on this day.";
                    return;
                }

                // Get existing appointments for the doctor on the selected date
                var existingAppointments = await _unitOfWork.Appointments.FindAsync(a => 
                    a.DoctorId == SelectedDoctor.Id && 
                    a.AppointmentDate.Date == AppointmentDate.Date && 
                    a.Status != AppointmentStatus.Cancelled);

                // Generate time slots (assuming 30-minute appointments)
                var timeSlots = new List<string>();
                var startTime = doctorSchedule.StartTime;
                var endTime = doctorSchedule.EndTime;

                while (startTime.Add(TimeSpan.FromMinutes(30)) <= endTime)
                {
                    var slotEndTime = startTime.Add(TimeSpan.FromMinutes(30));
                    var timeSlot = $"{startTime:hh\\:mm tt} - {slotEndTime:hh\\:mm tt}";

                    // Check if the time slot is already booked
                    bool isBooked = existingAppointments.Any(a => 
                        (a.StartTime <= startTime && a.EndTime > startTime) || 
                        (a.StartTime < slotEndTime && a.EndTime >= slotEndTime) ||
                        (a.StartTime >= startTime && a.EndTime <= slotEndTime));

                    // If editing an existing appointment, don't consider it as booked
                    if (_isEditMode && _appointmentId.HasValue)
                    {
                        isBooked = isBooked && !existingAppointments.Any(a => 
                            a.Id == _appointmentId && 
                            a.StartTime == startTime && 
                            a.EndTime == slotEndTime);
                    }

                    if (!isBooked)
                    {
                        timeSlots.Add(timeSlot);
                    }

                    startTime = slotEndTime;
                }

                AvailableTimeSlots.Clear();
                foreach (var slot in timeSlots)
                {
                    AvailableTimeSlots.Add(slot);
                }

                // If there are available slots, select the first one by default
                if (AvailableTimeSlots.Count > 0 && SelectedTimeSlot == null)
                {
                    SelectedTimeSlot = AvailableTimeSlots[0];
                }
                else if (AvailableTimeSlots.Count == 0)
                {
                    ErrorMessage = "No available time slots for the selected date and doctor.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading available time slots: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void SaveAppointmentAsync()
        {
            if (!ValidateInput())
            {
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                if (_isEditMode)
                {
                    await UpdateAppointmentAsync();
                }
                else
                {
                    await CreateAppointmentAsync();
                }

                _navigationService.NavigateTo<AppointmentListViewModel>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error saving appointment: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CreateAppointmentAsync()
        {
            if (SelectedPatient == null || SelectedDoctor == null || string.IsNullOrEmpty(SelectedTimeSlot))
            {
                return;
            }

            // Parse time slot
            var times = SelectedTimeSlot.Split('-');
            if (times.Length != 2)
            {
                ErrorMessage = "Invalid time slot format.";
                return;
            }

            var startTime = TimeSpan.Parse(times[0].Trim());
            var endTime = TimeSpan.Parse(times[1].Trim());

            // Create appointment
            var appointment = new Appointment
            {
                PatientId = SelectedPatient.Id,
                DoctorId = SelectedDoctor.Id,
                AppointmentDate = AppointmentDate,
                StartTime = startTime,
                EndTime = endTime,
                Purpose = Purpose,
                Notes = Notes,
                Status = AppointmentStatus.Scheduled,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.CompleteAsync();

            _dialogService.ShowMessage("Appointment has been scheduled successfully.", "Success");
        }

        private async Task UpdateAppointmentAsync()
        {
            if (!_appointmentId.HasValue || SelectedPatient == null || SelectedDoctor == null || string.IsNullOrEmpty(SelectedTimeSlot))
            {
                return;
            }

            var appointment = await _unitOfWork.Appointments.GetByIdAsync(_appointmentId.Value);
            if (appointment == null)
            {
                ErrorMessage = "Appointment not found.";
                return;
            }

            // Parse time slot
            var times = SelectedTimeSlot.Split('-');
            if (times.Length != 2)
            {
                ErrorMessage = "Invalid time slot format.";
                return;
            }

            var startTime = TimeSpan.Parse(times[0].Trim());
            var endTime = TimeSpan.Parse(times[1].Trim());

            // Update appointment
            appointment.PatientId = SelectedPatient.Id;
            appointment.DoctorId = SelectedDoctor.Id;
            appointment.AppointmentDate = AppointmentDate;
            appointment.StartTime = startTime;
            appointment.EndTime = endTime;
            appointment.Purpose = Purpose;
            appointment.Notes = Notes;
            appointment.Status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), Status);
            appointment.UpdatedAt = DateTime.Now;

            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.CompleteAsync();

            _dialogService.ShowMessage("Appointment has been updated successfully.", "Success");
        }

        private bool ValidateInput()
        {
            if (SelectedPatient == null)
            {
                ErrorMessage = "Please select a patient.";
                return false;
            }

            if (SelectedDoctor == null)
            {
                ErrorMessage = "Please select a doctor.";
                return false;
            }

            if (AppointmentDate < DateTime.Today)
            {
                ErrorMessage = "Appointment date cannot be in the past.";
                return false;
            }

            if (string.IsNullOrEmpty(SelectedTimeSlot))
            {
                ErrorMessage = "Please select a time slot.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Purpose))
            {
                ErrorMessage = "Purpose is required.";
                return false;
            }

            return true;
        }

        private void Cancel()
        {
            if (_isEditMode)
            {
                // Reload the appointment data
                if (_appointmentId.HasValue)
                {
                    LoadAppointmentAsync(_appointmentId.Value);
                }
            }
            else
            {
                // Clear the form
                SelectedPatient = null;
                SelectedDoctor = Doctors.FirstOrDefault();
                AppointmentDate = DateTime.Today.AddDays(1);
                SelectedTimeSlot = null;
                Purpose = string.Empty;
                Notes = string.Empty;
            }

            ErrorMessage = string.Empty;
        }
    }

    public class PatientViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class DoctorViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
    }
}
