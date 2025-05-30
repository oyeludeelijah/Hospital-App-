using HospitalApp.WPF.Commands;
using HospitalApp.WPF.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HospitalApp.WPF.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        public LoginViewModel(
            IAuthenticationService authenticationService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            LoginCommand = new RelayCommand<PasswordBox>(LoginAsync);
            ForgotPasswordCommand = new RelayCommand<object>(ForgotPassword);
        }

        private async void LoginAsync(PasswordBox? passwordBox)
        {
            if (passwordBox == null) return;

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Username is required";
                return;
            }

            if (string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                ErrorMessage = "Password is required";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                bool success = await _authenticationService.LoginAsync(Username, passwordBox.Password);

                if (success)
                {
                    // Navigate to dashboard
                    _navigationService.NavigateTo<DashboardViewModel>();
                }
                else
                {
                    ErrorMessage = "Invalid username or password";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ForgotPassword(object? parameter)
        {
            _dialogService.ShowMessage("Please contact the system administrator to reset your password.", "Password Recovery");
        }
    }
}
