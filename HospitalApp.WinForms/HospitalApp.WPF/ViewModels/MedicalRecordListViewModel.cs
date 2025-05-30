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
    public class MedicalRecordListViewModel : ViewModelBase
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
                    LoadMedicalRecordsAsync();
                }
            }
        }

        private PatientViewModel? _selectedPatientFilter;
        public PatientViewModel? SelectedPatientFilter
        {
            get => _selectedPatientFilter;
            set
            {
                if (SetProperty(ref _selectedPatientFilter, value))
                {
                    LoadMedicalRecordsAsync();
                }
            }
        }

        private MedicalRecordViewModel? _selectedMedicalRecord;
        public MedicalRecordViewModel? SelectedMedicalRecord
        {
            get => _selectedMedicalRecord;
            set => SetProperty(ref _selectedMedicalRecord, value);
        }

        public ObservableCollection<MedicalRecordViewModel> MedicalRecords { get; } = new ObservableCollection<MedicalRecordViewModel>();
        public ObservableCollection<PatientViewModel> Patients { get; } = new ObservableCollection<PatientViewModel>();

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    LoadMedicalRecordsAsync();
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
                    LoadMedicalRecordsAsync();
                    OnPropertyChanged(nameof(PaginationInfo));
                }
            }
        }

        private int _totalRecords;
        public int TotalRecords
        {
            get => _totalRecords;
            set
            {
                if (SetProperty(ref _totalRecords, value))
                {
                    OnPropertyChanged(nameof(PaginationInfo));
                    OnPropertyChanged(nameof(CanGoToNextPage));
                }
            }
        }

        public string PaginationInfo => $"Page {CurrentPage} of {Math.Max(1, (int)Math.Ceiling((double)TotalRecords / PageSize))}";
        public bool CanGoToPreviousPage => CurrentPage > 1;
        public bool CanGoToNextPage => CurrentPage < Math.Ceiling((double)TotalRecords / PageSize);
        public bool HasNoResults => !IsBusy && MedicalRecords.Count == 0;

        public ICommand AddNewRecordCommand { get; }
        public ICommand ViewRecordCommand { get; }
        public ICommand EditRecordCommand { get; }
        public ICommand DeleteRecordCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public MedicalRecordListViewModel(
            IUnitOfWork unitOfWork,
            INavigationService navigationService,
            IDialogService dialogService,
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _authenticationService = authenticationService;

            AddNewRecordCommand = new RelayCommand<object>(_ => NavigateToMedicalRecordDetails());
            ViewRecordCommand = new RelayCommand<MedicalRecordViewModel>(ViewMedicalRecord);
            EditRecordCommand = new RelayCommand<MedicalRecordViewModel>(EditMedicalRecord);
            DeleteRecordCommand = new RelayCommand<MedicalRecordViewModel>(DeleteMedicalRecord);
            FilterCommand = new RelayCommand<object>(_ => LoadMedicalRecordsAsync());
            PreviousPageCommand = new RelayCommand<object>(_ => CurrentPage--, _ => CanGoToPreviousPage);
            NextPageCommand = new RelayCommand<object>(_ => CurrentPage++, _ => CanGoToNextPage);

            LoadPatientsAsync();
            LoadMedicalRecordsAsync();
        }

        private async void LoadPatientsAsync()
        {
            try
            {
                var patients = await _unitOfWork.Patients.GetAllAsync();
                
                Patients.Clear();
                
                // Add "All Patients" option
                Patients.Add(new PatientViewModel { Id = 0, FullName = "All Patients" });
                
                foreach (var patient in patients)
                {
                    Patients.Add(new PatientViewModel
                    {
                        Id = patient.Id,
                        FullName = patient.User.FullName
                    });
                }

                // Select "All Patients" by default
                SelectedPatientFilter = Patients.First();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading patients: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
        }

        private async void LoadMedicalRecordsAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                // Calculate skip for pagination
                int skip = (CurrentPage - 1) * PageSize;

                // Build the filter predicate
                Func<MedicalRecord, bool> predicate = mr =>
                    (string.IsNullOrEmpty(SearchText) ||
                     mr.Patient.User.FirstName.Contains(SearchText) ||
                     mr.Patient.User.LastName.Contains(SearchText) ||
                     mr.Doctor.User.FirstName.Contains(SearchText) ||
                     mr.Doctor.User.LastName.Contains(SearchText) ||
                     mr.Diagnosis.Contains(SearchText) ||
                     mr.Symptoms.Contains(SearchText) ||
                     mr.Treatment.Contains(SearchText)) &&
                    (SelectedPatientFilter == null || SelectedPatientFilter.Id == 0 || mr.PatientId == SelectedPatientFilter.Id);

                // Get total count for pagination
                TotalRecords = await _unitOfWork.MedicalRecords.CountAsync(predicate);

                // Get medical records for current page
                var medicalRecords = await _unitOfWork.MedicalRecords.FindAsync(predicate);

                // Apply pagination
                medicalRecords = medicalRecords.Skip(skip).Take(PageSize).ToList();

                MedicalRecords.Clear();
                foreach (var record in medicalRecords)
                {
                    MedicalRecords.Add(new MedicalRecordViewModel
                    {
                        Id = record.Id,
                        PatientId = record.PatientId,
                        PatientName = record.Patient.User.FullName,
                        DoctorId = record.DoctorId,
                        DoctorName = record.Doctor.User.FullName,
                        NurseId = record.NurseId,
                        NurseName = record.Nurse?.User.FullName,
                        RecordDate = record.RecordDate,
                        Diagnosis = record.Diagnosis,
                        Symptoms = record.Symptoms,
                        Treatment = record.Treatment,
                        Notes = record.Notes,
                        IsConfidential = record.IsConfidential
                    });
                }

                OnPropertyChanged(nameof(HasNoResults));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading medical records: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void NavigateToMedicalRecordDetails(int? recordId = null)
        {
            if (recordId.HasValue)
            {
                _navigationService.NavigateTo<MedicalRecordDetailsViewModel>(recordId.Value);
            }
            else
            {
                _navigationService.NavigateTo<MedicalRecordDetailsViewModel>();
            }
        }

        private void ViewMedicalRecord(MedicalRecordViewModel? record)
        {
            if (record == null) return;
            NavigateToMedicalRecordDetails(record.Id);
        }

        private void EditMedicalRecord(MedicalRecordViewModel? record)
        {
            if (record == null) return;
            NavigateToMedicalRecordDetails(record.Id);
        }

        private async void DeleteMedicalRecord(MedicalRecordViewModel? record)
        {
            if (record == null) return;

            bool confirmed = await _dialogService.ShowConfirmationAsync(
                $"Are you sure you want to delete this medical record? This action cannot be undone.",
                "Confirm Delete");

            if (!confirmed) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var medicalRecord = await _unitOfWork.MedicalRecords.GetByIdAsync(record.Id);
                if (medicalRecord == null)
                {
                    _dialogService.ShowError("Medical record not found.");
                    return;
                }

                _unitOfWork.MedicalRecords.Remove(medicalRecord);
                await _unitOfWork.CompleteAsync();

                MedicalRecords.Remove(record);
                _dialogService.ShowMessage("Medical record has been deleted successfully.", "Success");

                // Refresh the list
                LoadMedicalRecordsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting medical record: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    public class MedicalRecordViewModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int? NurseId { get; set; }
        public string? NurseName { get; set; }
        public DateTime RecordDate { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool IsConfidential { get; set; }
    }

    public class PatientViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
