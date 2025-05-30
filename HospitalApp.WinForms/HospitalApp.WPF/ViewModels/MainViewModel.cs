using HospitalApp.WPF.Commands;
using HospitalApp.WPF.Services;
using HospitalApp.WPF.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HospitalApp.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IDialogService _dialogService;

        private object _currentView = null!;
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public bool IsUserLoggedIn => _authenticationService.IsAuthenticated;
        
        public string CurrentUserName => _authenticationService.CurrentUser?.FullName ?? string.Empty;

        public ICommand NavigateToCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _authenticationService = authenticationService;
            _dialogService = dialogService;

            NavigateToCommand = new RelayCommand<string>(NavigateTo);
            LogoutCommand = new RelayCommand<object>(Logout);

            _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;

            // Start with login view if not authenticated
            if (!_authenticationService.IsAuthenticated)
            {
                _navigationService.NavigateTo<LoginViewModel>();
            }
            else
            {
                _navigationService.NavigateTo<DashboardViewModel>();
            }
        }

        private void NavigateTo(string? destination)
        {
            if (string.IsNullOrEmpty(destination)) return;

            switch (destination)
            {
                case "Dashboard":
                    _navigationService.NavigateTo<DashboardViewModel>();
                    break;
                case "PatientList":
                    _navigationService.NavigateTo<PatientListViewModel>();
                    break;
                case "PatientDetails":
                    _navigationService.NavigateTo<PatientDetailsViewModel>();
                    break;
                case "AppointmentList":
                    _navigationService.NavigateTo<AppointmentListViewModel>();
                    break;
                case "AppointmentDetails":
                    _navigationService.NavigateTo<AppointmentDetailsViewModel>();
                    break;
                case "DoctorList":
                    _navigationService.NavigateTo<DoctorListViewModel>();
                    break;
                case "DoctorDetails":
                    _navigationService.NavigateTo<DoctorDetailsViewModel>();
                    break;
                case "MedicalRecordList":
                    _navigationService.NavigateTo<MedicalRecordListViewModel>();
                    break;
                case "MedicalRecordDetails":
                    _navigationService.NavigateTo<MedicalRecordDetailsViewModel>();
                    break;
                // Add other navigation destinations as needed
                default:
                    _dialogService.ShowWarning($"Navigation to {destination} is not implemented yet.");
                    break;
            }
        }

        private void Logout(object? parameter)
        {
            _authenticationService.Logout();
            _navigationService.NavigateTo<LoginViewModel>();
            OnPropertyChanged(nameof(IsUserLoggedIn));
            OnPropertyChanged(nameof(CurrentUserName));
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentView = _navigationService.CurrentViewModel;
            OnPropertyChanged(nameof(IsUserLoggedIn));
            OnPropertyChanged(nameof(CurrentUserName));
        }
    }
}
