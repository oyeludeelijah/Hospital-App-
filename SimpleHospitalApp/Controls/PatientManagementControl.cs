using SimpleHospitalApp.Controls;
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
    public class PatientManagementControl : BaseManagementControl
    {
        public PatientManagementControl()
        {
            // Set title specific to this control
            lblTitle.Text = "Patient Management";
            btnAdd.Text = "Add Patient";
            txtSearch.PlaceholderText = "Search by name...";
            
            // Load initial data
            LoadPatients();
        }
        
        protected override void InitializeBaseComponent()
        {
            // Call the base implementation first
            base.InitializeBaseComponent();
            
            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(dataGridView);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
        }
        
        private void InitializeComponent()
        {
            // Setup control
            this.Size = new Size(940, 400);
            
            // Create title label
            lblTitle = new Label();
            lblTitle.Text = "Patient Management";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Size = new Size(300, 30);
            
            // Create search controls
            txtSearch = new TextBox();
            txtSearch.PlaceholderText = "Search by name...";
            txtSearch.Location = new Point(530, 10);
            txtSearch.Size = new Size(200, 25);
            
            btnSearch = new Button();
            btnSearch.Text = "Find";
            btnSearch.Location = new Point(740, 10);
            btnSearch.Size = new Size(60, 25);
            btnSearch.Click += (s, e) => Search();
            
            // Create DataGridView
            dataGridView = new DataGridView();
            dataGridView.Location = new Point(10, 50);
            dataGridView.Size = new Size(810, 280);
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = false;
            dataGridView.ReadOnly = true;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.RowHeadersVisible = false;
            dataGridView.BackgroundColor = Color.White;
            
            // Create buttons
            btnAdd = new Button();
            btnAdd.Text = "Add Patient";
            btnAdd.Location = new Point(830, 50);
            btnAdd.Size = new Size(100, 30);
            btnAdd.BackColor = SystemColors.ButtonFace;
            btnAdd.Click += (s, e) => Add();
            
            btnEdit = new Button();
            btnEdit.Text = "Edit";
            btnEdit.Location = new Point(830, 90);
            btnEdit.Size = new Size(100, 30);
            btnEdit.BackColor = SystemColors.ButtonFace;
            btnEdit.Click += (s, e) => Edit();
            
            btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.Location = new Point(830, 130);
            btnDelete.Size = new Size(100, 30);
            btnDelete.BackColor = SystemColors.ButtonFace;
            btnDelete.Click += (s, e) => Delete();
        }
        
        private void LoadPatients()
        {
            try
            {
                var patients = DataService.Instance.GetAllPatients();
                
                dataGridView.DataSource = new BindingList<PatientViewModel>(
                    patients.Select(p => new PatientViewModel
                    {
                        Id = p.Id,
                        FullName = $"{p.FirstName} {p.LastName}",
                        DateOfBirth = p.DateOfBirth,
                        Gender = p.Gender,
                        ContactNumber = p.ContactNumber,
                        Email = p.Email,
                        Address = p.Address
                    }).ToList());
                
                if (dataGridView.Columns.Count > 0)
                {
                    dataGridView.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowError("loading patients", ex);
            }
        }
        
        /// <summary>
        /// Implements the abstract Search method from the base class.
        /// Filters the patient list based on search criteria.
        /// </summary>
        protected override void Search()
        {
            string searchTerm = txtSearch.Text.Trim().ToLower();
            var patients = DataService.Instance.GetAllPatients();
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                patients = patients.Where(p => 
                    (p.FirstName ?? string.Empty).ToLower().Contains(searchTerm) || 
                    (p.LastName ?? string.Empty).ToLower().Contains(searchTerm) ||
                    (p.ContactNumber ?? string.Empty).ToLower().Contains(searchTerm) ||
                    (p.Email ?? string.Empty).ToLower().Contains(searchTerm)).ToList();
            }
            
            dataGridView.DataSource = new BindingList<PatientViewModel>(
                patients.Select(p => new PatientViewModel
                {
                    Id = p.Id,
                    FullName = $"{p.FirstName} {p.LastName}",
                    DateOfBirth = p.DateOfBirth,
                    Gender = p.Gender,
                    ContactNumber = p.ContactNumber,
                    Email = p.Email,
                    Address = p.Address
                }).ToList());
        }
        
        /// <summary>
        /// Implements the abstract Add method from the base class.
        /// Opens the form to add a new patient.
        /// </summary>
        protected override void Add()
        {
            using (var form = new PatientForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadPatients();
                }
            }
        }
        
        /// <summary>
        /// Implements the abstract Edit method from the base class.
        /// Opens the form to edit the selected patient.
        /// </summary>
        protected override void Edit()
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView.SelectedRows[0];
                int patientId = (int)selectedRow.Cells["Id"].Value;
                
                var patient = DataService.Instance.GetPatientById(patientId);
                if (patient != null)
                {
                    using (var form = new PatientForm(patient))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            LoadPatients();
                        }
                    }
                }
            }
            else
            {
                ShowSelectionRequired("patient");
            }
        }
        
        /// <summary>
        /// Implements the abstract Delete method from the base class.
        /// Deletes the selected patient after confirmation and validation.
        /// </summary>
        protected override void Delete()
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView.SelectedRows[0];
                int patientId = (int)selectedRow.Cells["Id"].Value;
                string patientName = selectedRow.Cells["FullName"].Value.ToString();
                
                // Check if patient has appointments
                var patientAppointments = DataService.Instance.GetAppointmentsByPatient(patientId);
                if (patientAppointments.Count > 0)
                {
                    MessageBox.Show($"Cannot delete patient '{patientName}' because they have {patientAppointments.Count} appointments scheduled. " +
                        $"Please cancel these appointments first.", "Cannot Delete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Check if patient has medical records
                var patientRecords = DataService.Instance.GetMedicalRecordsByPatient(patientId);
                if (patientRecords.Count > 0)
                {
                    MessageBox.Show($"Cannot delete patient '{patientName}' because they have {patientRecords.Count} medical records. " +
                        $"Please delete these records first.", "Cannot Delete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Check if patient has billings
                var patientBillings = DataService.Instance.GetBillingsByPatient(patientId);
                if (patientBillings.Count > 0)
                {
                    MessageBox.Show($"Cannot delete patient '{patientName}' because they have {patientBillings.Count} billing records. " +
                        $"Please delete these billing records first.", "Cannot Delete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                var result = ConfirmDelete(patientName, "patient");
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        DataService.Instance.DeletePatient(patientId);
                        LoadPatients();
                    }
                    catch (Exception ex)
                    {
                        ShowError("deleting patient", ex);
                    }
                }
            }
            else
            {
                ShowSelectionRequired("patient");
            }
        }
        
        // View model for the DataGridView
        public class PatientViewModel
        {
            public int Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public DateTime DateOfBirth { get; set; }
            public string Gender { get; set; } = string.Empty;
            public string ContactNumber { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
        }
    }
}
