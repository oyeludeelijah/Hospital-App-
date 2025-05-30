using HospitalApp.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace HospitalApp.WPF.Services
{
    public interface INavigationService
    {
        ViewModelBase CurrentViewModel { get; }
        event Action? CurrentViewModelChanged;
        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
        void NavigateTo<TViewModel>(object parameter) where TViewModel : ViewModelBase;
        void GoBack();
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack<ViewModelBase> _navigationStack = new Stack<ViewModelBase>();
        private ViewModelBase _currentViewModel = null!;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke();
            }
        }

        public event Action? CurrentViewModelChanged;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            var viewModel = _serviceProvider.GetService(typeof(TViewModel)) as ViewModelBase;
            
            if (viewModel == null)
            {
                throw new InvalidOperationException($"ViewModel of type {typeof(TViewModel).Name} could not be resolved.");
            }

            if (CurrentViewModel != null)
            {
                _navigationStack.Push(CurrentViewModel);
            }

            CurrentViewModel = viewModel;
        }

        public void NavigateTo<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            var viewModel = _serviceProvider.GetService(typeof(TViewModel)) as ViewModelBase;
            
            if (viewModel == null)
            {
                throw new InvalidOperationException($"ViewModel of type {typeof(TViewModel).Name} could not be resolved.");
            }

            if (viewModel is IParameterizedViewModel parameterizedViewModel)
            {
                parameterizedViewModel.Initialize(parameter);
            }

            if (CurrentViewModel != null)
            {
                _navigationStack.Push(CurrentViewModel);
            }

            CurrentViewModel = viewModel;
        }

        public void GoBack()
        {
            if (_navigationStack.Count > 0)
            {
                CurrentViewModel = _navigationStack.Pop();
            }
        }
    }

    public interface IParameterizedViewModel
    {
        void Initialize(object parameter);
    }
}
