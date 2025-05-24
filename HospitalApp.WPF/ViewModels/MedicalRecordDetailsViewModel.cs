using HospitalApp.Core.Models;
using HospitalApp.Data;
using HospitalApp.WPF.Commands;
using HospitalApp.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HospitalApp.WPF.ViewModels
{
    public class MedicalRecordDetailsViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IAuthenticationService _authenticationService;

        private int? _medicalRecordId;
        private bool _isNewRecord;

        public string PageTitle => _isNewRecord ? "New Medical Record" : "Edit Medical Record";

        private StaffViewModel? _selectedPatient;
        public StaffViewModel? SelectedPatient
        {
            get => _selectedPatient;
            set => SetProperty(ref _selectedPatient, value);
        }

        private StaffViewModel? _selectedDoctor;
        public StaffViewModel? SelectedDoctor
        {
            get => _selectedDoctor;
            set => SetProperty(ref _selectedDoctor, value);
        }

        private StaffViewModel? _selectedNurse;
        public StaffViewModel? SelectedNurse
        {
            get => _selectedNurse;
            set => SetProperty(ref _selectedNurse, value);
        }

        private DateTime _recordDate = DateTime.Today;
        public DateTime RecordDate
        {
            get => _recordDate;
            set => SetProperty(ref _recordDate, value);
        }

        private string _diagnosis = string.Empty;
        public string Diagnosis
        {
            get => _diagnosis;
            set => SetProperty(ref _diagnosis, value);
        }

        private string _symptoms = string.Empty;
        public string Symptoms
        {
            get => _symptoms;
            set => SetProperty(ref _symptoms, value);
        }

        private string _treatment = string.Empty;
        public string Treatment
        {
            get => _treatment;
            set => SetProperty(ref _treatment, value);
        }

        private string _notes = string.Empty;
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        private bool _isConfidential;
        public bool IsConfidential
        {
            get => _isConfidential;
            set => SetProperty(ref _isConfidential, value);
        }

        public ObservableCollection<StaffViewModel> Patients { get; } = new ObservableCollection<StaffViewModel>();
        public ObservableCollection<StaffViewModel> Doctors { get; } = new ObservableCollection<StaffViewModel>();
        public ObservableCollection<StaffViewModel> Nurses { get; } = new ObservableCollection<StaffViewModel>();

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BackToListCommand { get; }

        public MedicalRecordDetailsViewModel(
            IUnitOfWork unitOfWork,
            INavigationService navigationService,
            IDialogService dialogService,
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _authenticationService = authenticationService;

            SaveCommand = new RelayCommand<object>(_ => SaveMedicalRecordAsync());
            CancelCommand = new RelayCommand<object>(_ => NavigateBack());
            BackToListCommand = new RelayCommand<object>(_ => NavigateBack());

            // Check if we're editing an existing record or creating a new one
            if (navigationService.Parameter != null && navigationService.Parameter is int recordId)
            {
                _medicalRecordId = recordId;
                _isNewRecord = false;
                LoadMedicalRecordAsync(recordId);
            }
            else
            {
                _isNewRecord = true;
                // Set default values for a new record
                RecordDate = DateTime.Today;
            }

            // Load staff data
            LoadPatientsAsync();
            LoadDoctorsAsync();
            LoadNursesAsync();
        }

        private async void LoadPatientsAsync()
        {
            try
            {
                var patients = await _unitOfWork.Patients.GetAllAsync();
                
                Patients.Clear();
                foreach (var patient in patients)
                {
                    Patients.Add(new StaffViewModel
                    {
                        Id = patient.Id,
                        FullName = patient.User.FullName
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading patients: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
        }

        private async void LoadDoctorsAsync()
        {
            try
            {
                var doctors = await _unitOfWork.Doctors.GetAllAsync();
                
                Doctors.Clear();
                foreach (var doctor in doctors)
                {
                    Doctors.Add(new StaffViewModel
                    {
                        Id = doctor.Id,
                        FullName = doctor.User.FullName
                    });
                }

                // If the current user is a doctor, select them by default for new records
                if (_isNewRecord && _authenticationService.CurrentUser?.Role == "Doctor")
                {
                    var currentDoctor = await _unitOfWork.Doctors.FindAsync(d => d.UserId == _authenticationService.CurrentUser.Id);
                    var doctor = currentDoctor.FirstOrDefault();
                    if (doctor != null)
                    {
                        SelectedDoctor = Doctors.FirstOrDefault(d => d.Id == doctor.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading doctors: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
        }

        private async void LoadNursesAsync()
        {
            try
            {
                var nurses = await _unitOfWork.Nurses.GetAllAsync();
                
                Nurses.Clear();
                // Add empty option for nurse (since it's optional)
                Nurses.Add(new StaffViewModel { Id = 0, FullName = "-- None --" });
                
                foreach (var nurse in nurses)
                {
                    Nurses.Add(new StaffViewModel
                    {
                        Id = nurse.Id,
                        FullName = nurse.User.FullName
                    });
                }

                // Select "None" by default
                SelectedNurse = Nurses.First();

                // If the current user is a nurse, select them by default for new records
                if (_isNewRecord && _authenticationService.CurrentUser?.Role == "Nurse")
                {
                    var currentNurse = await _unitOfWork.Nurses.FindAsync(n => n.UserId == _authenticationService.CurrentUser.Id);
                    var nurse = currentNurse.FirstOrDefault();
                    if (nurse != null)
                    {
                        SelectedNurse = Nurses.FirstOrDefault(n => n.Id == nurse.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading nurses: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
        }

        private async void LoadMedicalRecordAsync(int recordId)
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var medicalRecord = await _unitOfWork.MedicalRecords.GetByIdAsync(recordId);
                if (medicalRecord == null)
                {
                    _dialogService.ShowError("Medical record not found.");
                    NavigateBack();
                    return;
                }

                // Wait for staff data to be loaded
                await Task.Delay(500);

                // Populate form fields
                SelectedPatient = Patients.FirstOrDefault(p => p.Id == medicalRecord.PatientId);
                SelectedDoctor = Doctors.FirstOrDefault(d => d.Id == medicalRecord.DoctorId);
                SelectedNurse = medicalRecord.NurseId.HasValue 
                    ? Nurses.FirstOrDefault(n => n.Id == medicalRecord.NurseId.Value) 
                    : Nurses.First(); // None option
                RecordDate = medicalRecord.RecordDate;
                Diagnosis = medicalRecord.Diagnosis;
                Symptoms = medicalRecord.Symptoms;
                Treatment = medicalRecord.Treatment;
                Notes = medicalRecord.Notes;
                IsConfidential = medicalRecord.IsConfidential;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading medical record: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
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

            if (string.IsNullOrWhiteSpace(Diagnosis))
            {
                ErrorMessage = "Diagnosis is required.";
                return false;
            }

            return true;
        }

        private async void SaveMedicalRecordAsync()
        {
            if (!ValidateInput())
            {
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                MedicalRecord medicalRecord;

                if (_isNewRecord)
                {
                    // Create new medical record
                    medicalRecord = new MedicalRecord
                    {
                        PatientId = SelectedPatient!.Id,
                        DoctorId = SelectedDoctor!.Id,
                        NurseId = SelectedNurse?.Id > 0 ? SelectedNurse.Id : null,
                        RecordDate = RecordDate,
                        Diagnosis = Diagnosis,
                        Symptoms = Symptoms,
                        Treatment = Treatment,
                        Notes = Notes,
                        IsConfidential = IsConfidential,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _authenticationService.CurrentUser?.Id ?? 0
                    };

                    _unitOfWork.MedicalRecords.Add(medicalRecord);
                }
                else
                {
                    // Update existing medical record
                    medicalRecord = await _unitOfWork.MedicalRecords.GetByIdAsync(_medicalRecordId!.Value);
                    if (medicalRecord == null)
                    {
                        _dialogService.ShowError("Medical record not found.");
                        return;
                    }

                    medicalRecord.PatientId = SelectedPatient!.Id;
                    medicalRecord.DoctorId = SelectedDoctor!.Id;
                    medicalRecord.NurseId = SelectedNurse?.Id > 0 ? SelectedNurse.Id : null;
                    medicalRecord.RecordDate = RecordDate;
                    medicalRecord.Diagnosis = Diagnosis;
                    medicalRecord.Symptoms = Symptoms;
                    medicalRecord.Treatment = Treatment;
                    medicalRecord.Notes = Notes;
                    medicalRecord.IsConfidential = IsConfidential;
                    medicalRecord.UpdatedAt = DateTime.Now;
                    medicalRecord.UpdatedBy = _authenticationService.CurrentUser?.Id ?? 0;

                    _unitOfWork.MedicalRecords.Update(medicalRecord);
                }

                await _unitOfWork.CompleteAsync();

                _dialogService.ShowMessage(
                    _isNewRecord 
                        ? "Medical record has been created successfully." 
                        : "Medical record has been updated successfully.", 
                    "Success");

                // Navigate back to the list
                NavigateBack();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error saving medical record: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void NavigateBack()
        {
            _navigationService.NavigateTo<MedicalRecordListViewModel>();
        }
    }

    public class StaffViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
