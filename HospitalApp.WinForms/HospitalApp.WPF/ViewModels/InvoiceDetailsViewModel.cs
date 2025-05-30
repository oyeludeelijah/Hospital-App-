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
    public class InvoiceDetailsViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IAuthenticationService _authenticationService;

        private int? _invoiceId;
        private bool _isNewInvoice;
        public bool IsNewInvoice => _isNewInvoice;
        public bool IsEditMode => !_isNewInvoice;

        public string PageTitle => _isNewInvoice ? "New Invoice" : "Edit Invoice";

        private string _invoiceNumber = string.Empty;
        public string InvoiceNumber
        {
            get => _invoiceNumber;
            set => SetProperty(ref _invoiceNumber, value);
        }

        private PatientViewModel? _selectedPatient;
        public PatientViewModel? SelectedPatient
        {
            get => _selectedPatient;
            set => SetProperty(ref _selectedPatient, value);
        }

        private DateTime _invoiceDate = DateTime.Today;
        public DateTime InvoiceDate
        {
            get => _invoiceDate;
            set => SetProperty(ref _invoiceDate, value);
        }

        private DateTime _dueDate = DateTime.Today.AddDays(30);
        public DateTime DueDate
        {
            get => _dueDate;
            set => SetProperty(ref _dueDate, value);
        }

        private string _status = "Pending";
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _notes = string.Empty;
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        private decimal _subtotal;
        public decimal Subtotal
        {
            get => _subtotal;
            set
            {
                if (SetProperty(ref _subtotal, value))
                {
                    CalculateTotals();
                }
            }
        }

        private decimal _tax;
        public decimal Tax
        {
            get => _tax;
            set
            {
                if (SetProperty(ref _tax, value))
                {
                    CalculateTotals();
                }
            }
        }

        private decimal _totalAmount;
        public decimal TotalAmount
        {
            get => _totalAmount;
            set => SetProperty(ref _totalAmount, value);
        }

        public ObservableCollection<PatientViewModel> Patients { get; } = new ObservableCollection<PatientViewModel>();
        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string>
        {
            "Pending",
            "Paid",
            "Overdue",
            "Cancelled"
        };
        public ObservableCollection<InvoiceLineItemViewModel> LineItems { get; } = new ObservableCollection<InvoiceLineItemViewModel>();

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BackToListCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand RemoveItemCommand { get; }

        public InvoiceDetailsViewModel(
            IUnitOfWork unitOfWork,
            INavigationService navigationService,
            IDialogService dialogService,
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _authenticationService = authenticationService;

            SaveCommand = new RelayCommand<object>(_ => SaveInvoiceAsync());
            CancelCommand = new RelayCommand<object>(_ => NavigateBack());
            BackToListCommand = new RelayCommand<object>(_ => NavigateBack());
            AddItemCommand = new RelayCommand<object>(_ => AddLineItem());
            EditItemCommand = new RelayCommand<InvoiceLineItemViewModel>(EditLineItem);
            RemoveItemCommand = new RelayCommand<InvoiceLineItemViewModel>(RemoveLineItem);

            // Check if we're editing an existing invoice or creating a new one
            if (navigationService.Parameter != null && navigationService.Parameter is int invoiceId)
            {
                _invoiceId = invoiceId;
                _isNewInvoice = false;
                LoadInvoiceAsync(invoiceId);
            }
            else
            {
                _isNewInvoice = true;
                // Set default values for a new invoice
                InvoiceDate = DateTime.Today;
                DueDate = DateTime.Today.AddDays(30);
                Status = "Pending";
                GenerateInvoiceNumber();
            }

            // Load patients
            LoadPatientsAsync();
        }

        private async void LoadPatientsAsync()
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading patients: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
        }

        private void GenerateInvoiceNumber()
        {
            // Generate a unique invoice number based on date and time
            string dateString = DateTime.Now.ToString("yyyyMMdd");
            string timeString = DateTime.Now.ToString("HHmmss");
            InvoiceNumber = $"INV-{dateString}-{timeString}";
        }

        private async void LoadInvoiceAsync(int invoiceId)
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var bill = await _unitOfWork.Bills.GetByIdAsync(invoiceId);
                if (bill == null)
                {
                    _dialogService.ShowError("Invoice not found.");
                    NavigateBack();
                    return;
                }

                // Wait for patients data to be loaded
                await Task.Delay(500);

                // Populate form fields
                InvoiceNumber = bill.InvoiceNumber;
                SelectedPatient = Patients.FirstOrDefault(p => p.Id == bill.PatientId);
                InvoiceDate = bill.InvoiceDate;
                DueDate = bill.DueDate;
                Status = bill.Status;
                Description = bill.Description;
                Notes = bill.Notes;

                // Load line items
                var lineItems = await _unitOfWork.BillItems.FindAsync(item => item.BillId == invoiceId);
                
                LineItems.Clear();
                foreach (var item in lineItems)
                {
                    LineItems.Add(new InvoiceLineItemViewModel
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Total = item.Quantity * item.UnitPrice
                    });
                }

                CalculateTotals();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading invoice: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CalculateTotals()
        {
            Subtotal = LineItems.Sum(item => item.Total);
            Tax = Math.Round(Subtotal * 0.1m, 2); // Assuming 10% tax rate
            TotalAmount = Subtotal + Tax;
        }

        private async void AddLineItem()
        {
            try
            {
                var result = await _dialogService.ShowLineItemDialogAsync(null);
                if (result != null)
                {
                    LineItems.Add(result);
                    CalculateTotals();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error adding line item: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
        }

        private async void EditLineItem(InvoiceLineItemViewModel? lineItem)
        {
            if (lineItem == null) return;

            try
            {
                var result = await _dialogService.ShowLineItemDialogAsync(lineItem);
                if (result != null)
                {
                    int index = LineItems.IndexOf(lineItem);
                    if (index >= 0)
                    {
                        LineItems[index] = result;
                        CalculateTotals();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error editing line item: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
        }

        private void RemoveLineItem(InvoiceLineItemViewModel? lineItem)
        {
            if (lineItem == null) return;

            LineItems.Remove(lineItem);
            CalculateTotals();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(InvoiceNumber))
            {
                ErrorMessage = "Invoice number is required.";
                return false;
            }

            if (SelectedPatient == null)
            {
                ErrorMessage = "Please select a patient.";
                return false;
            }

            if (LineItems.Count == 0)
            {
                ErrorMessage = "Please add at least one item to the invoice.";
                return false;
            }

            if (DueDate < InvoiceDate)
            {
                ErrorMessage = "Due date cannot be earlier than invoice date.";
                return false;
            }

            return true;
        }

        private async void SaveInvoiceAsync()
        {
            if (!ValidateInput())
            {
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                Bill bill;

                if (_isNewInvoice)
                {
                    // Create new invoice
                    bill = new Bill
                    {
                        InvoiceNumber = InvoiceNumber,
                        PatientId = SelectedPatient!.Id,
                        InvoiceDate = InvoiceDate,
                        DueDate = DueDate,
                        Status = Status,
                        Description = Description,
                        Notes = Notes,
                        Subtotal = Subtotal,
                        Tax = Tax,
                        TotalAmount = TotalAmount,
                        AmountPaid = 0,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _authenticationService.CurrentUser?.Id ?? 0
                    };

                    _unitOfWork.Bills.Add(bill);
                    await _unitOfWork.CompleteAsync();

                    // Add line items
                    foreach (var item in LineItems)
                    {
                        var billItem = new BillItem
                        {
                            BillId = bill.Id,
                            Description = item.Description,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _authenticationService.CurrentUser?.Id ?? 0
                        };

                        _unitOfWork.BillItems.Add(billItem);
                    }
                }
                else
                {
                    // Update existing invoice
                    bill = await _unitOfWork.Bills.GetByIdAsync(_invoiceId!.Value);
                    if (bill == null)
                    {
                        _dialogService.ShowError("Invoice not found.");
                        return;
                    }

                    // Update invoice details
                    bill.PatientId = SelectedPatient!.Id;
                    bill.InvoiceDate = InvoiceDate;
                    bill.DueDate = DueDate;
                    bill.Status = Status;
                    bill.Description = Description;
                    bill.Notes = Notes;
                    bill.Subtotal = Subtotal;
                    bill.Tax = Tax;
                    bill.TotalAmount = TotalAmount;
                    bill.UpdatedAt = DateTime.Now;
                    bill.UpdatedBy = _authenticationService.CurrentUser?.Id ?? 0;

                    _unitOfWork.Bills.Update(bill);

                    // Delete existing line items
                    var existingItems = await _unitOfWork.BillItems.FindAsync(item => item.BillId == bill.Id);
                    foreach (var item in existingItems)
                    {
                        _unitOfWork.BillItems.Remove(item);
                    }

                    // Add new line items
                    foreach (var item in LineItems)
                    {
                        var billItem = new BillItem
                        {
                            BillId = bill.Id,
                            Description = item.Description,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _authenticationService.CurrentUser?.Id ?? 0
                        };

                        _unitOfWork.BillItems.Add(billItem);
                    }
                }

                await _unitOfWork.CompleteAsync();

                _dialogService.ShowMessage(
                    _isNewInvoice 
                        ? "Invoice has been created successfully." 
                        : "Invoice has been updated successfully.", 
                    "Success");

                // Navigate back to the list
                NavigateBack();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error saving invoice: {ex.Message}";
                _dialogService.ShowError(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void NavigateBack()
        {
            _navigationService.NavigateTo<BillingListViewModel>();
        }
    }

    public class PatientViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class InvoiceLineItemViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }
}
