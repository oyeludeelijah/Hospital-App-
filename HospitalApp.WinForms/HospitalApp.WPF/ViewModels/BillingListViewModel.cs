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
    public class BillingListViewModel : ViewModelBase
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
                    LoadInvoicesAsync();
                }
            }
        }

        private string _selectedStatusFilter = "All";
        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                if (SetProperty(ref _selectedStatusFilter, value))
                {
                    LoadInvoicesAsync();
                }
            }
        }

        private string _selectedDateRangeFilter = "All Time";
        public string SelectedDateRangeFilter
        {
            get => _selectedDateRangeFilter;
            set
            {
                if (SetProperty(ref _selectedDateRangeFilter, value))
                {
                    LoadInvoicesAsync();
                }
            }
        }

        private InvoiceViewModel? _selectedInvoice;
        public InvoiceViewModel? SelectedInvoice
        {
            get => _selectedInvoice;
            set => SetProperty(ref _selectedInvoice, value);
        }

        public ObservableCollection<InvoiceViewModel> Invoices { get; } = new ObservableCollection<InvoiceViewModel>();
        public ObservableCollection<string> StatusFilters { get; } = new ObservableCollection<string>
        {
            "All",
            "Pending",
            "Paid",
            "Overdue",
            "Cancelled"
        };

        public ObservableCollection<string> DateRangeFilters { get; } = new ObservableCollection<string>
        {
            "All Time",
            "Today",
            "This Week",
            "This Month",
            "Last 3 Months",
            "Last 6 Months",
            "This Year"
        };

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    LoadInvoicesAsync();
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
                    LoadInvoicesAsync();
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
        public bool HasNoResults => !IsBusy && Invoices.Count == 0;

        public ICommand AddNewInvoiceCommand { get; }
        public ICommand ViewInvoiceCommand { get; }
        public ICommand EditInvoiceCommand { get; }
        public ICommand RecordPaymentCommand { get; }
        public ICommand PrintInvoiceCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public BillingListViewModel(
            IUnitOfWork unitOfWork,
            INavigationService navigationService,
            IDialogService dialogService,
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _authenticationService = authenticationService;

            AddNewInvoiceCommand = new RelayCommand<object>(_ => NavigateToInvoiceDetails());
            ViewInvoiceCommand = new RelayCommand<InvoiceViewModel>(ViewInvoice);
            EditInvoiceCommand = new RelayCommand<InvoiceViewModel>(EditInvoice);
            RecordPaymentCommand = new RelayCommand<InvoiceViewModel>(RecordPayment);
            PrintInvoiceCommand = new RelayCommand<InvoiceViewModel>(PrintInvoice);
            FilterCommand = new RelayCommand<object>(_ => LoadInvoicesAsync());
            PreviousPageCommand = new RelayCommand<object>(_ => CurrentPage--, _ => CanGoToPreviousPage);
            NextPageCommand = new RelayCommand<object>(_ => CurrentPage++, _ => CanGoToNextPage);

            LoadInvoicesAsync();
        }

        private async void LoadInvoicesAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                // Calculate skip for pagination
                int skip = (CurrentPage - 1) * PageSize;

                // Build the date range filter
                DateTime? startDate = null;
                DateTime endDate = DateTime.Now;

                switch (SelectedDateRangeFilter)
                {
                    case "Today":
                        startDate = DateTime.Today;
                        break;
                    case "This Week":
                        startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                        break;
                    case "This Month":
                        startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        break;
                    case "Last 3 Months":
                        startDate = DateTime.Today.AddMonths(-3);
                        break;
                    case "Last 6 Months":
                        startDate = DateTime.Today.AddMonths(-6);
                        break;
                    case "This Year":
                        startDate = new DateTime(DateTime.Today.Year, 1, 1);
                        break;
                }

                // Build the filter predicate
                Func<Bill, bool> predicate = bill =>
                    (string.IsNullOrEmpty(SearchText) ||
                     bill.InvoiceNumber.Contains(SearchText) ||
                     bill.Patient.User.FirstName.Contains(SearchText) ||
                     bill.Patient.User.LastName.Contains(SearchText)) &&
                    (SelectedStatusFilter == "All" || bill.Status == SelectedStatusFilter) &&
                    (!startDate.HasValue || bill.InvoiceDate >= startDate.Value) &&
                    (bill.InvoiceDate <= endDate);

                // Get total count for pagination
                TotalRecords = await _unitOfWork.Bills.CountAsync(predicate);

                // Get invoices for current page
                var bills = await _unitOfWork.Bills.FindAsync(predicate);

                // Apply pagination
                bills = bills.Skip(skip).Take(PageSize).ToList();

                Invoices.Clear();
                foreach (var bill in bills)
                {
                    Invoices.Add(new InvoiceViewModel
                    {
                        Id = bill.Id,
                        InvoiceNumber = bill.InvoiceNumber,
                        PatientId = bill.PatientId,
                        PatientName = bill.Patient.User.FullName,
                        InvoiceDate = bill.InvoiceDate,
                        DueDate = bill.DueDate,
                        TotalAmount = bill.TotalAmount,
                        AmountPaid = bill.AmountPaid,
                        Balance = bill.TotalAmount - bill.AmountPaid,
                        Status = bill.Status,
                        IsPayable = bill.Status != "Paid" && bill.Status != "Cancelled"
                    });
                }

                OnPropertyChanged(nameof(HasNoResults));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading invoices: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void NavigateToInvoiceDetails(int? invoiceId = null)
        {
            if (invoiceId.HasValue)
            {
                _navigationService.NavigateTo<InvoiceDetailsViewModel>(invoiceId.Value);
            }
            else
            {
                _navigationService.NavigateTo<InvoiceDetailsViewModel>();
            }
        }

        private void ViewInvoice(InvoiceViewModel? invoice)
        {
            if (invoice == null) return;
            NavigateToInvoiceDetails(invoice.Id);
        }

        private void EditInvoice(InvoiceViewModel? invoice)
        {
            if (invoice == null) return;
            
            if (invoice.Status == "Paid" || invoice.Status == "Cancelled")
            {
                _dialogService.ShowError("Cannot edit a paid or cancelled invoice.");
                return;
            }
            
            NavigateToInvoiceDetails(invoice.Id);
        }

        private async void RecordPayment(InvoiceViewModel? invoice)
        {
            if (invoice == null) return;
            
            if (!invoice.IsPayable)
            {
                _dialogService.ShowError("Cannot record payment for this invoice.");
                return;
            }

            _navigationService.NavigateTo<PaymentDetailsViewModel>(invoice.Id);
        }

        private async void PrintInvoice(InvoiceViewModel? invoice)
        {
            if (invoice == null) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                // In a real application, this would generate a PDF or print the invoice
                // For now, we'll just show a message
                _dialogService.ShowMessage($"Invoice #{invoice.InvoiceNumber} has been sent to the printer.", "Print Invoice");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error printing invoice: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    public class InvoiceViewModel
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPayable { get; set; }
    }
}
