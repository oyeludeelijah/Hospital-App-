using SimpleHospitalApp.Forms;
using SimpleHospitalApp.Models;
using SimpleHospitalApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleHospitalApp
{
    public class DepartmentManagementControl : UserControl
    {
        private DataGridView dgvDepartments;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Label lblTitle;
        private TextBox txtSearch;
        private Button btnSearch;
        
        public DepartmentManagementControl()
        {
            InitializeComponent();
            LoadDepartments();
        }
        
        private void InitializeComponent()
        {
            // Setup control
            this.Size = new Size(940, 400);
            
            // Create title label
            lblTitle = new Label();
            lblTitle.Text = "Department Management";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Size = new Size(300, 30);
            
            // Create search controls
            txtSearch = new TextBox();
            txtSearch.Location = new Point(530, 10);
            txtSearch.Size = new Size(200, 25);
            txtSearch.PlaceholderText = "Search departments...";
            
            btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Location = new Point(740, 10);
            btnSearch.Size = new Size(80, 25);
            btnSearch.Click += (s, e) => SearchDepartments();
            
            // Create DataGridView
            dgvDepartments = new DataGridView();
            dgvDepartments.Location = new Point(10, 50);
            dgvDepartments.Size = new Size(810, 280);
            dgvDepartments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDepartments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDepartments.MultiSelect = false;
            dgvDepartments.ReadOnly = true;
            dgvDepartments.AllowUserToAddRows = false;
            dgvDepartments.AllowUserToDeleteRows = false;
            dgvDepartments.AllowUserToResizeRows = false;
            dgvDepartments.RowHeadersVisible = false;
            dgvDepartments.BackgroundColor = Color.White;
            
            // Create buttons
            btnAdd = new Button();
            btnAdd.Text = "Add Department";
            btnAdd.Location = new Point(830, 50);
            btnAdd.Size = new Size(100, 30);
            btnAdd.Click += (s, e) => AddDepartment();
            
            btnEdit = new Button();
            btnEdit.Text = "Edit";
            btnEdit.Location = new Point(830, 90);
            btnEdit.Size = new Size(100, 30);
            btnEdit.Click += (s, e) => EditDepartment();
            
            btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.Location = new Point(830, 130);
            btnDelete.Size = new Size(100, 30);
            btnDelete.Click += (s, e) => DeleteDepartment();
            
            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(dgvDepartments);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
        }
        
        private void LoadDepartments()
        {
            try
            {
                var departments = DataService.Instance.GetAllDepartments();
                
                var viewModels = departments.Select(d => new DepartmentViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    DoctorCount = DataService.Instance.GetDoctorsByDepartment(d.Id).Count
                }).ToList();
                
                dgvDepartments.DataSource = new BindingList<DepartmentViewModel>(viewModels);
                
                if (dgvDepartments.Columns.Count > 0)
                {
                    dgvDepartments.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading departments: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void SearchDepartments()
        {
            string searchTerm = txtSearch.Text.Trim().ToLower();
            var departments = DataService.Instance.GetAllDepartments();
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                departments = departments.Where(d => 
                    (d.Name ?? string.Empty).ToLower().Contains(searchTerm) || 
                    (d.Description ?? string.Empty).ToLower().Contains(searchTerm)).ToList();
            }
            
            var viewModels = departments.Select(d => new DepartmentViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                DoctorCount = DataService.Instance.GetDoctorsByDepartment(d.Id).Count
            }).ToList();
            
            dgvDepartments.DataSource = new BindingList<DepartmentViewModel>(viewModels);
        }
        
        private void AddDepartment()
        {
            var departmentForm = new DepartmentForm();
            if (departmentForm.ShowDialog() == DialogResult.OK)
            {
                LoadDepartments();
            }
        }
        
        private void EditDepartment()
        {
            if (dgvDepartments.SelectedRows.Count > 0)
            {
                int departmentId = (int)dgvDepartments.SelectedRows[0].Cells["Id"].Value;
                var department = DataService.Instance.GetDepartmentById(departmentId);
                
                if (department != null)
                {
                    var departmentForm = new DepartmentForm(department);
                    if (departmentForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadDepartments();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a department to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void DeleteDepartment()
        {
            if (dgvDepartments.SelectedRows.Count > 0)
            {
                int departmentId = (int)dgvDepartments.SelectedRows[0].Cells["Id"].Value;
                string departmentName = dgvDepartments.SelectedRows[0].Cells["Name"].Value.ToString();
                
                // Check if department has doctors
                var doctorsInDepartment = DataService.Instance.GetDoctorsByDepartment(departmentId);
                if (doctorsInDepartment.Count > 0)
                {
                    MessageBox.Show($"Cannot delete department '{departmentName}' because it has {doctorsInDepartment.Count} doctors assigned to it. " +
                        $"Please reassign the doctors to other departments first.", "Cannot Delete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                var result = MessageBox.Show($"Are you sure you want to delete the department '{departmentName}'?", 
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    DataService.Instance.DeleteDepartment(departmentId);
                    MessageBox.Show("Department deleted successfully.", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDepartments();
                }
            }
            else
            {
                MessageBox.Show("Please select a department to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
    
    public class DepartmentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DoctorCount { get; set; }
    }
}
