using HospitalApp.Core.Models;
using HospitalApp.Data;
using HospitalApp.WPF.Commands;
using HospitalApp.WPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HospitalApp.WPF.ViewModels
{
    public class PatientDetailsViewModel : ViewModelBase, IParameterizedViewModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IAuthenticationService _authenticationService;

        private int? _patientId;
        private bool _isEditMode;

        public string PageTitle => _isEditMode ? "Edit Patient" : "Add New Patient";

        #region Patient Properties

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        private DateTime _dateOfBirth = DateTime.Today.AddYears(-30);
        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set => SetProperty(ref _dateOfBirth, value);
        }

        private string _gender = "Male";
        public string Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        public List<string> GenderOptions { get; } = new List<string> { "Male", "Female", "Other" };

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _address = string.Empty;
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private string _emergencyContactName = string.Empty;
        public string EmergencyContactName
        {
            get => _emergencyContactName;
            set => SetProperty(ref _emergencyContactName, value);
        }

        private string _emergencyContactPhone = string.Empty;
        public string EmergencyContactPhone
        {
            get => _emergencyContactPhone;
            set => SetProperty(ref _emergencyContactPhone, value);
        }

        private string _bloodType = "O+";
        public string BloodType
        {
            get => _bloodType;
            set => SetProperty(ref _bloodType, value);
        }

        public List<string> BloodTypeOptions { get; } = new List<string> { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-", "Unknown" };

        private bool _hasInsurance;
        public bool HasInsurance
        {
            get => _hasInsurance;
            set => SetProperty(ref _hasInsurance, value);
        }

        private string _insuranceProvider = string.Empty;
        public string InsuranceProvider
        {
            get => _insuranceProvider;
            set => SetProperty(ref _insuranceProvider, value);
        }

        private string _insuranceNumber = string.Empty;
        public string InsuranceNumber
        {
            get => _insuranceNumber;
            set => SetProperty(ref _insuranceNumber, value);
        }

        #endregion

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BackToListCommand { get; }

        public PatientDetailsViewModel(
            IUnitOfWork unitOfWork,
            INavigationService navigationService,
            IDialogService dialogService,
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _authenticationService = authenticationService;

            SaveCommand = new RelayCommand<object>(_ => SavePatientAsync());
            CancelCommand = new RelayCommand<object>(_ => Cancel());
            BackToListCommand = new RelayCommand<object>(_ => _navigationService.NavigateTo<PatientListViewModel>());
        }

        public void Initialize(object parameter)
        {
            if (parameter is int patientId)
            {
                _patientId = patientId;
                _isEditMode = true;
                LoadPatientAsync(patientId);
            }
            else
            {
                _patientId = null;
                _isEditMode = false;
            }

            OnPropertyChanged(nameof(PageTitle));
        }

        private async void LoadPatientAsync(int patientId)
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var patient = await _unitOfWork.Patients.GetByIdAsync(patientId);
                if (patient == null)
                {
                    _dialogService.ShowError("Patient not found.");
                    _navigationService.NavigateTo<PatientListViewModel>();
                    return;
                }

                // Load patient data
                FirstName = patient.User.FirstName;
                LastName = patient.User.LastName;
                DateOfBirth = patient.DateOfBirth;
                Gender = patient.Gender.ToString();
                PhoneNumber = patient.User.PhoneNumber;
                Email = patient.User.Email;
                Address = patient.Address;
                EmergencyContactName = patient.EmergencyContactName;
                EmergencyContactPhone = patient.EmergencyContactPhone;
                BloodType = patient.BloodType;
                HasInsurance = patient.HasInsurance;
                InsuranceProvider = patient.InsuranceProvider ?? string.Empty;
                InsuranceNumber = patient.InsuranceNumber ?? string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading patient: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void SavePatientAsync()
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
                    await UpdatePatientAsync();
                }
                else
                {
                    await CreatePatientAsync();
                }

                _navigationService.NavigateTo<PatientListViewModel>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error saving patient: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CreatePatientAsync()
        {
            // Create user account
            var user = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                Email = Email,
                Username = GenerateUsername(FirstName, LastName),
                PasswordHash = "DefaultPassword123", // In a real app, this would be properly hashed
                Role = UserRole.Patient,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            // Create patient record
            var patient = new Patient
            {
                UserId = user.Id,
                PatientNumber = GeneratePatientNumber(),
                DateOfBirth = DateOfBirth,
                Gender = (Gender)Enum.Parse(typeof(Gender), Gender),
                Address = Address,
                EmergencyContactName = EmergencyContactName,
                EmergencyContactPhone = EmergencyContactPhone,
                BloodType = BloodType,
                HasInsurance = HasInsurance,
                InsuranceProvider = HasInsurance ? InsuranceProvider : null,
                InsuranceNumber = HasInsurance ? InsuranceNumber : null,
                RegisteredDate = DateTime.Now
            };

            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.CompleteAsync();

            _dialogService.ShowMessage($"Patient {FirstName} {LastName} has been created successfully.", "Success");
        }

        private async Task UpdatePatientAsync()
        {
            if (!_patientId.HasValue) return;

            var patient = await _unitOfWork.Patients.GetByIdAsync(_patientId.Value);
            if (patient == null) return;

            var user = await _unitOfWork.Users.GetByIdAsync(patient.UserId);
            if (user == null) return;

            // Update user information
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.PhoneNumber = PhoneNumber;
            user.Email = Email;

            // Update patient information
            patient.DateOfBirth = DateOfBirth;
            patient.Gender = (Gender)Enum.Parse(typeof(Gender), Gender);
            patient.Address = Address;
            patient.EmergencyContactName = EmergencyContactName;
            patient.EmergencyContactPhone = EmergencyContactPhone;
            patient.BloodType = BloodType;
            patient.HasInsurance = HasInsurance;
            patient.InsuranceProvider = HasInsurance ? InsuranceProvider : null;
            patient.InsuranceNumber = HasInsurance ? InsuranceNumber : null;

            _unitOfWork.Users.Update(user);
            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.CompleteAsync();

            _dialogService.ShowMessage($"Patient {FirstName} {LastName} has been updated successfully.", "Success");
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                ErrorMessage = "First name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                ErrorMessage = "Last name is required.";
                return false;
            }

            if (DateOfBirth > DateTime.Today)
            {
                ErrorMessage = "Date of birth cannot be in the future.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                ErrorMessage = "Phone number is required.";
                return false;
            }

            if (HasInsurance)
            {
                if (string.IsNullOrWhiteSpace(InsuranceProvider))
                {
                    ErrorMessage = "Insurance provider is required when 'Has Insurance' is checked.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(InsuranceNumber))
                {
                    ErrorMessage = "Insurance number is required when 'Has Insurance' is checked.";
                    return false;
                }
            }

            return true;
        }

        private void Cancel()
        {
            if (_isEditMode)
            {
                // Reload the patient data
                if (_patientId.HasValue)
                {
                    LoadPatientAsync(_patientId.Value);
                }
            }
            else
            {
                // Clear the form
                FirstName = string.Empty;
                LastName = string.Empty;
                DateOfBirth = DateTime.Today.AddYears(-30);
                Gender = "Male";
                PhoneNumber = string.Empty;
                Email = string.Empty;
                Address = string.Empty;
                EmergencyContactName = string.Empty;
                EmergencyContactPhone = string.Empty;
                BloodType = "O+";
                HasInsurance = false;
                InsuranceProvider = string.Empty;
                InsuranceNumber = string.Empty;
            }

            ErrorMessage = string.Empty;
        }

        private string GenerateUsername(string firstName, string lastName)
        {
            // Generate a username based on first name and last name
            string baseUsername = $"{firstName.ToLower()}.{lastName.ToLower()}";
            
            // In a real app, you would check if the username already exists and add a number if needed
            return baseUsername;
        }

        private string GeneratePatientNumber()
        {
            // Generate a unique patient number
            // In a real app, this would be more sophisticated and ensure uniqueness
            return $"P{DateTime.Now:yyMMddHHmmss}";
        }
    }
}
