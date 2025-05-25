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
    public class DoctorManagementControl : UserControl
    {
        private DataGridView dgvDoctors;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Label lblTitle;
        private TextBox txtSearch;
        private Button btnSearch;
        
        public DoctorManagementControl()
        {
            InitializeComponent();
            LoadDoctors();
        }
        
        private void InitializeComponent()
        {
            // Setup control
            this.Size = new Size(940, 400);
            
            // Create title label
            lblTitle = new Label();
            lblTitle.Text = "Doctor Management";
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
            btnSearch.Click += (s, e) => SearchDoctors();
            
            // Create DataGridView
            dgvDoctors = new DataGridView();
            dgvDoctors.Location = new Point(10, 50);
            dgvDoctors.Size = new Size(810, 280);
            dgvDoctors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDoctors.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDoctors.MultiSelect = false;
            dgvDoctors.ReadOnly = true;
            dgvDoctors.AllowUserToAddRows = false;
            dgvDoctors.AllowUserToDeleteRows = false;
            dgvDoctors.AllowUserToResizeRows = false;
            dgvDoctors.RowHeadersVisible = false;
            dgvDoctors.BackgroundColor = Color.White;
            
            // Create buttons
            btnAdd = new Button();
            btnAdd.Text = "Add Doctor";
            btnAdd.Location = new Point(830, 50);
            btnAdd.Size = new Size(110, 30);
            btnAdd.BackColor = SystemColors.ButtonFace;
            btnAdd.Click += (s, e) => AddDoctor();
            
            btnEdit = new Button();
            btnEdit.Text = "Edit";
            btnEdit.Location = new Point(830, 90);
            btnEdit.Size = new Size(100, 30);
            btnEdit.BackColor = SystemColors.ButtonFace;
            btnEdit.Click += (s, e) => EditDoctor();
            
            btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.Location = new Point(830, 130);
            btnDelete.Size = new Size(100, 30);
            btnDelete.BackColor = SystemColors.ButtonFace;
            btnDelete.Click += (s, e) => DeleteDoctor();
            
            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(dgvDoctors);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
        }
        
        private void LoadDoctors()
        {
            var doctors = DataService.Instance.GetAllDoctors();
            
            dgvDoctors.DataSource = null;
            dgvDoctors.Columns.Clear();
            
            var bindingList = new BindingList<DoctorViewModel>(
                doctors.Select(d => new DoctorViewModel
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Specialization = d.Specialization,
                    ContactNumber = d.ContactNumber,
                    Email = d.Email
                }).ToList());
            
            dgvDoctors.DataSource = bindingList;
            
            if (dgvDoctors.Columns.Count > 0)
            {
                dgvDoctors.Columns["Id"].Visible = false;
            }
        }
        
        private void SearchDoctors()
        {
            string searchTerm = txtSearch.Text.Trim().ToLower();
            var doctors = DataService.Instance.GetAllDoctors();
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                doctors = doctors.Where(d => 
                    d.FirstName.ToLower().Contains(searchTerm) || 
                    d.LastName.ToLower().Contains(searchTerm) ||
                    d.Specialization.ToLower().Contains(searchTerm)).ToList();
            }
            
            dgvDoctors.DataSource = new BindingList<DoctorViewModel>(
                doctors.Select(d => new DoctorViewModel
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Specialization = d.Specialization,
                    ContactNumber = d.ContactNumber,
                    Email = d.Email
                }).ToList());
        }
        
        private void AddDoctor()
        {
            using (var form = new DoctorForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadDoctors();
                }
            }
        }
        
        private void EditDoctor()
        {
            if (dgvDoctors.SelectedRows.Count > 0)
            {
                var selectedRow = dgvDoctors.SelectedRows[0];
                int doctorId = (int)selectedRow.Cells["Id"].Value;
                
                var doctor = DataService.Instance.GetDoctorById(doctorId);
                if (doctor != null)
                {
                    using (var form = new DoctorForm(doctor))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            LoadDoctors();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a doctor to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void DeleteDoctor()
        {
            if (dgvDoctors.SelectedRows.Count > 0)
            {
                var selectedRow = dgvDoctors.SelectedRows[0];
                int doctorId = (int)selectedRow.Cells["Id"].Value;
                string doctorName = (string)selectedRow.Cells["FullName"].Value;
                
                var result = MessageBox.Show($"Are you sure you want to delete doctor '{doctorName}'?", 
                    "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    DataService.Instance.DeleteDoctor(doctorId);
                    LoadDoctors();
                }
            }
            else
            {
                MessageBox.Show("Please select a doctor to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
    
    // View model for the DataGridView
    public class DoctorViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
