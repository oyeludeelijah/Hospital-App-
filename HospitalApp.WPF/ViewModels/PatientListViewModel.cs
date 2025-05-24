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
    public class PatientListViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    LoadPatientsAsync();
                }
            }
        }

        private PatientViewModel? _selectedPatient;
        public PatientViewModel? SelectedPatient
        {
            get => _selectedPatient;
            set => SetProperty(ref _selectedPatient, value);
        }

        public ObservableCollection<PatientViewModel> Patients { get; } = new ObservableCollection<PatientViewModel>();

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    LoadPatientsAsync();
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
                    LoadPatientsAsync();
                    OnPropertyChanged(nameof(PaginationInfo));
                }
            }
        }

        private int _totalPatients;
        public int TotalPatients
        {
            get => _totalPatients;
            set
            {
                if (SetProperty(ref _totalPatients, value))
                {
                    OnPropertyChanged(nameof(PaginationInfo));
                    OnPropertyChanged(nameof(CanGoToNextPage));
                }
            }
        }

        public string PaginationInfo => $"Page {CurrentPage} of {Math.Max(1, (int)Math.Ceiling((double)TotalPatients / PageSize))}";
        public bool CanGoToPreviousPage => CurrentPage > 1;
        public bool CanGoToNextPage => CurrentPage < Math.Ceiling((double)TotalPatients / PageSize);
        public bool HasNoResults => !IsBusy && Patients.Count == 0;

        public ICommand AddNewPatientCommand { get; }
        public ICommand ViewPatientCommand { get; }
        public ICommand EditPatientCommand { get; }
        public ICommand DeletePatientCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public PatientListViewModel(
            IUnitOfWork unitOfWork,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _unitOfWork = unitOfWork;
            _navigationService = navigationService;
            _dialogService = dialogService;

            AddNewPatientCommand = new RelayCommand<object>(_ => NavigateToPatientDetails());
            ViewPatientCommand = new RelayCommand<PatientViewModel>(ViewPatient);
            EditPatientCommand = new RelayCommand<PatientViewModel>(EditPatient);
            DeletePatientCommand = new RelayCommand<PatientViewModel>(DeletePatient);
            FilterCommand = new RelayCommand<object>(_ => ShowFilterDialog());
            PreviousPageCommand = new RelayCommand<object>(_ => CurrentPage--, _ => CanGoToPreviousPage);
            NextPageCommand = new RelayCommand<object>(_ => CurrentPage++, _ => CanGoToNextPage);

            LoadPatientsAsync();
        }

        private async void LoadPatientsAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                // Calculate skip for pagination
                int skip = (CurrentPage - 1) * PageSize;

                // Get total count for pagination
                TotalPatients = await _unitOfWork.Patients.CountAsync(p => 
                    string.IsNullOrEmpty(SearchText) || 
                    p.User.FirstName.Contains(SearchText) || 
                    p.User.LastName.Contains(SearchText) || 
                    p.PatientNumber.Contains(SearchText) || 
                    p.User.PhoneNumber.Contains(SearchText));

                // Get patients for current page
                var patients = await _unitOfWork.Patients.FindAsync(p => 
                    string.IsNullOrEmpty(SearchText) || 
                    p.User.FirstName.Contains(SearchText) || 
                    p.User.LastName.Contains(SearchText) || 
                    p.PatientNumber.Contains(SearchText) || 
                    p.User.PhoneNumber.Contains(SearchText));

                // Apply pagination
                patients = patients.Skip(skip).Take(PageSize).ToList();

                Patients.Clear();
                foreach (var patient in patients)
                {
                    Patients.Add(new PatientViewModel
                    {
                        Id = patient.Id,
                        PatientNumber = patient.PatientNumber,
                        FirstName = patient.User.FirstName,
                        LastName = patient.User.LastName,
                        FullName = patient.User.FullName,
                        Gender = patient.Gender.ToString(),
                        Age = patient.Age,
                        DateOfBirth = patient.DateOfBirth,
                        PhoneNumber = patient.User.PhoneNumber,
                        Email = patient.User.Email,
                        Address = patient.Address,
                        RegisteredDate = patient.RegisteredDate,
                        HasInsurance = patient.HasInsurance,
                        InsuranceProvider = patient.InsuranceProvider,
                        InsuranceNumber = patient.InsuranceNumber
                    });
                }

                OnPropertyChanged(nameof(HasNoResults));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading patients: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void NavigateToPatientDetails(int? patientId = null)
        {
            if (patientId.HasValue)
            {
                _navigationService.NavigateTo<PatientDetailsViewModel>(patientId.Value);
            }
            else
            {
                _navigationService.NavigateTo<PatientDetailsViewModel>();
            }
        }

        private void ViewPatient(PatientViewModel? patient)
        {
            if (patient == null) return;
            NavigateToPatientDetails(patient.Id);
        }

        private void EditPatient(PatientViewModel? patient)
        {
            if (patient == null) return;
            NavigateToPatientDetails(patient.Id);
        }

        private async void DeletePatient(PatientViewModel? patient)
        {
            if (patient == null) return;

            bool confirmed = await _dialogService.ShowConfirmationAsync(
                $"Are you sure you want to delete patient {patient.FullName}? This action cannot be undone.",
                "Confirm Delete");

            if (!confirmed) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var patientEntity = await _unitOfWork.Patients.GetByIdAsync(patient.Id);
                if (patientEntity == null)
                {
                    _dialogService.ShowError("Patient not found.");
                    return;
                }

                _unitOfWork.Patients.Remove(patientEntity);
                await _unitOfWork.CompleteAsync();

                Patients.Remove(patient);
                _dialogService.ShowMessage($"Patient {patient.FullName} has been deleted.", "Success");

                // Refresh the list
                LoadPatientsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting patient: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ShowFilterDialog()
        {
            _dialogService.ShowMessage("Advanced filtering will be implemented in a future update.", "Coming Soon");
        }
    }

    public class PatientViewModel
    {
        public int Id { get; set; }
        public string PatientNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime RegisteredDate { get; set; }
        public bool HasInsurance { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? InsuranceNumber { get; set; }
    }
}
