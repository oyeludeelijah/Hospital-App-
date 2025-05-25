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
    public class PatientManagementControl : UserControl
    {
        private DataGridView dgvPatients;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Label lblTitle;
        private TextBox txtSearch;
        private Button btnSearch;
        
        public PatientManagementControl()
        {
            InitializeComponent();
            LoadPatients();
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
            btnSearch.Click += (s, e) => SearchPatients();
            
            // Create DataGridView
            dgvPatients = new DataGridView();
            dgvPatients.Location = new Point(10, 50);
            dgvPatients.Size = new Size(810, 280);
            dgvPatients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPatients.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPatients.MultiSelect = false;
            dgvPatients.ReadOnly = true;
            dgvPatients.AllowUserToAddRows = false;
            dgvPatients.AllowUserToDeleteRows = false;
            dgvPatients.AllowUserToResizeRows = false;
            dgvPatients.RowHeadersVisible = false;
            dgvPatients.BackgroundColor = Color.White;
            
            // Create buttons
            btnAdd = new Button();
            btnAdd.Text = "Add Patient";
            btnAdd.Location = new Point(830, 50);
            btnAdd.Size = new Size(110, 30);
            btnAdd.BackColor = SystemColors.ButtonFace;
            btnAdd.Click += (s, e) => AddPatient();
            
            btnEdit = new Button();
            btnEdit.Text = "Edit";
            btnEdit.Location = new Point(830, 90);
            btnEdit.Size = new Size(100, 30);
            btnEdit.BackColor = SystemColors.ButtonFace;
            btnEdit.Click += (s, e) => EditPatient();
            
            btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.Location = new Point(830, 130);
            btnDelete.Size = new Size(100, 30);
            btnDelete.BackColor = SystemColors.ButtonFace;
            btnDelete.Click += (s, e) => DeletePatient();
            
            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(dgvPatients);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
        }
        
        private void LoadPatients()
        {
            var patients = DataService.Instance.GetAllPatients();
            
            dgvPatients.DataSource = null;
            dgvPatients.Columns.Clear();
            
            var bindingList = new BindingList<PatientViewModel>(
                patients.Select(p => new PatientViewModel
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Age = p.Age,
                    Gender = p.Gender,
                    ContactNumber = p.ContactNumber,
                    Email = p.Email
                }).ToList());
            
            dgvPatients.DataSource = bindingList;
            
            if (dgvPatients.Columns.Count > 0)
            {
                dgvPatients.Columns["Id"].Visible = false;
            }
        }
        
        private void SearchPatients()
        {
            string searchTerm = txtSearch.Text.Trim().ToLower();
            var patients = DataService.Instance.GetAllPatients();
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                patients = patients.Where(p => 
                    (p.FirstName ?? string.Empty).ToLower().Contains(searchTerm) || 
                    (p.LastName ?? string.Empty).ToLower().Contains(searchTerm)).ToList();
            }
            
            dgvPatients.DataSource = new BindingList<PatientViewModel>(
                patients.Select(p => new PatientViewModel
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Age = p.Age,
                    Gender = p.Gender,
                    ContactNumber = p.ContactNumber,
                    Email = p.Email
                }).ToList());
        }
        
        private void AddPatient()
        {
            using (var form = new PatientForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadPatients();
                }
            }
        }
        
        private void EditPatient()
        {
            if (dgvPatients.SelectedRows.Count > 0)
            {
                var selectedRow = dgvPatients.SelectedRows[0];
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
                MessageBox.Show("Please select a patient to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void DeletePatient()
        {
            if (dgvPatients.SelectedRows.Count > 0)
            {
                var selectedRow = dgvPatients.SelectedRows[0];
                int patientId = (int)selectedRow.Cells["Id"].Value;
                string patientName = (string)selectedRow.Cells["FullName"].Value;
                
                var result = MessageBox.Show($"Are you sure you want to delete patient '{patientName}'?", 
                    "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    DataService.Instance.DeletePatient(patientId);
                    LoadPatients();
                }
            }
            else
            {
                MessageBox.Show("Please select a patient to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
    
    // View model for the DataGridView
    public class PatientViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
