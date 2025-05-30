using SimpleHospitalApp.Data;
using SimpleHospitalApp.Forms;
using SimpleHospitalApp.Models;
using SimpleHospitalApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleHospitalApp
{
    public class BillingManagementControl : UserControl
    {
        private DataGridView dgvBillings;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Label lblTitle;
        private ComboBox cmbFilterStatus;
        private Button btnFilter;
        private Button btnClearFilter;
        
        public BillingManagementControl()
        {
            InitializeComponent();
            LoadBillings();
        }
        
        private void InitializeComponent()
        {
            // Setup control
            this.Size = new Size(940, 400);
            
            // Create title label
            lblTitle = new Label();
            lblTitle.Text = "Billing Management";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Size = new Size(300, 30);
            
            // Create filter controls
            cmbFilterStatus = new ComboBox();
            cmbFilterStatus.Items.AddRange(new string[] { "All", "Pending", "Paid", "Overdue" });
            cmbFilterStatus.SelectedIndex = 0;
            cmbFilterStatus.Location = new Point(530, 10);
            cmbFilterStatus.Size = new Size(140, 25);
            cmbFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            
            btnFilter = new Button();
            btnFilter.Text = "Filter";
            btnFilter.Location = new Point(680, 10);
            btnFilter.Size = new Size(60, 25);
            btnFilter.Click += (s, e) => FilterBillings();
            
            btnClearFilter = new Button();
            btnClearFilter.Text = "Clear";
            btnClearFilter.Location = new Point(750, 10);
            btnClearFilter.Size = new Size(60, 25);
            btnClearFilter.Click += (s, e) => LoadBillings();
            
            // Create DataGridView
            dgvBillings = new DataGridView();
            dgvBillings.Location = new Point(10, 50);
            dgvBillings.Size = new Size(810, 280);
            dgvBillings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBillings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBillings.MultiSelect = false;
            dgvBillings.ReadOnly = true;
            dgvBillings.AllowUserToAddRows = false;
            dgvBillings.AllowUserToDeleteRows = false;
            dgvBillings.AllowUserToResizeRows = false;
            dgvBillings.RowHeadersVisible = false;
            dgvBillings.BackgroundColor = Color.White;
            
            // Create buttons
            btnAdd = new Button();
            btnAdd.Text = "Add Billing";
            btnAdd.Location = new Point(830, 50);
            btnAdd.Size = new Size(100, 30);
            btnAdd.BackColor = SystemColors.ButtonFace;
            btnAdd.Click += (s, e) => AddBilling();
            
            btnEdit = new Button();
            btnEdit.Text = "Edit";
            btnEdit.Location = new Point(830, 90);
            btnEdit.Size = new Size(100, 30);
            btnEdit.BackColor = SystemColors.ButtonFace;
            btnEdit.Click += (s, e) => EditBilling();
            
            btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.Location = new Point(830, 130);
            btnDelete.Size = new Size(100, 30);
            btnDelete.BackColor = SystemColors.ButtonFace;
            btnDelete.Click += (s, e) => DeleteBilling();
            
            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(cmbFilterStatus);
            this.Controls.Add(btnFilter);
            this.Controls.Add(btnClearFilter);
            this.Controls.Add(dgvBillings);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
        }
        
        private void LoadBillings()
        {
            try
            {
                // Get data from the DataService instead of direct database access
                var billings = DataService.Instance.GetAllBillings();
                var patients = DataService.Instance.GetAllPatients();
                
                // Create view models for the grid
                var viewModels = new List<BillingViewModel>();
                
                foreach (var billing in billings)
                {
                    var patient = patients.FirstOrDefault(p => p.Id == billing.PatientId);
                    viewModels.Add(new BillingViewModel
                    {
                        Id = billing.Id,
                        PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "Unknown",
                        Amount = billing.Amount,
                        BillingDate = billing.BillingDate,
                        PaymentStatus = billing.PaymentStatus ?? "Pending",
                        PaymentMethod = billing.PaymentMethod ?? string.Empty
                    });
                }
                
                // Sort by billing date (newest first)
                viewModels = viewModels.OrderByDescending(b => b.BillingDate).ToList();
                
                // Create a binding list for the DataGridView
                var bindingList = new BindingList<BillingViewModel>(viewModels);
                dgvBillings.DataSource = bindingList;
                
                // Configure grid appearance
                if (dgvBillings.Columns.Count > 0)
                {
                    dgvBillings.Columns["Id"].Visible = false;
                    
                    if (dgvBillings.Columns.Contains("Amount"))
                    {
                        dgvBillings.Columns["Amount"].DefaultCellStyle.Format = "C2";
                    }
                    
                    if (dgvBillings.Columns.Contains("BillingDate"))
                    {
                        dgvBillings.Columns["BillingDate"].DefaultCellStyle.Format = "MM/dd/yyyy";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading billing data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void FilterBillings()
        {
            string filterStatus = cmbFilterStatus.SelectedItem.ToString();
            
            try
            {
                // Get data from the DataService
                var billings = DataService.Instance.GetAllBillings();
                var patients = DataService.Instance.GetAllPatients();
                
                // Apply status filter if not "All"
                if (filterStatus != "All")
                {
                    billings = billings.Where(b => b.PaymentStatus == filterStatus).ToList();
                }
                
                // Create view models for the grid
                var viewModels = new List<BillingViewModel>();
                
                foreach (var billing in billings)
                {
                    var patient = patients.FirstOrDefault(p => p.Id == billing.PatientId);
                    viewModels.Add(new BillingViewModel
                    {
                        Id = billing.Id,
                        PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "Unknown",
                        Amount = billing.Amount,
                        BillingDate = billing.BillingDate,
                        PaymentStatus = billing.PaymentStatus ?? "Pending",
                        PaymentMethod = billing.PaymentMethod ?? string.Empty
                    });
                }
                
                // Sort by billing date (newest first)
                viewModels = viewModels.OrderByDescending(b => b.BillingDate).ToList();
                
                // Update the grid
                dgvBillings.DataSource = new BindingList<BillingViewModel>(viewModels);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering billing data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void AddBilling()
        {
            var billingForm = new BillingForm();
            if (billingForm.ShowDialog() == DialogResult.OK)
            {
                LoadBillings();
            }
        }
        
        private void EditBilling()
        {
            if (dgvBillings.SelectedRows.Count > 0)
            {
                int billingId = (int)dgvBillings.SelectedRows[0].Cells["Id"].Value;
                
                try
                {
                    // Get the billing from DataService instead of database
                    var billing = DataService.Instance.GetBillingById(billingId);
                    if (billing != null)
                    {
                        var billingForm = new BillingForm(billing);
                        if (billingForm.ShowDialog() == DialogResult.OK)
                        {
                            // Refresh the billing list after editing
                            LoadBillings();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error editing billing: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a billing record to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void DeleteBilling()
        {
            if (dgvBillings.SelectedRows.Count > 0)
            {
                int billingId = (int)dgvBillings.SelectedRows[0].Cells["Id"].Value;
                string patientName = dgvBillings.SelectedRows[0].Cells["PatientName"].Value.ToString();
                
                var result = MessageBox.Show($"Are you sure you want to delete the billing record for {patientName}?", 
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Delete using DataService instead of direct database access
                        DataService.Instance.DeleteBilling(billingId);
                        MessageBox.Show("Billing record deleted successfully.", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Refresh the billing list after deletion
                        LoadBillings();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting billing: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a billing record to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
    
    // View model for the DataGridView
    public class BillingViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime BillingDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
