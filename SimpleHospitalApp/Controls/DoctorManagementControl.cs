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
    public class DoctorManagementControl : BaseManagementControl
    {
        public DoctorManagementControl()
        {
            // Set title specific to this control
            lblTitle.Text = "Doctor Management";
            btnAdd.Text = "Add Doctor";
            txtSearch.PlaceholderText = "Search by name or specialization...";
            
            // Load initial data
            LoadDoctors();
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
        
        // InitializeComponent is now handled by the base class
        
        private void LoadDoctors()
        {
            try
            {
                var doctors = DataService.Instance.GetAllDoctors();
                dataGridView.DataSource = new BindingList<DoctorViewModel>(
                    doctors.Select(d => new DoctorViewModel
                    {
                        Id = d.Id,
                        FullName = d.FullName,
                        Specialization = d.Specialization,
                        ContactNumber = d.ContactNumber,
                        Email = d.Email
                    }).ToList());
                
                if (dataGridView.Columns.Count > 0)
                {
                    dataGridView.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowError("loading doctors", ex);
            }
        }
        
        protected override void Search()
        {
            string searchTerm = txtSearch.Text.Trim().ToLower();
            var doctors = DataService.Instance.GetAllDoctors();
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                doctors = doctors.Where(d => 
                    (d.FirstName ?? string.Empty).ToLower().Contains(searchTerm) || 
                    (d.LastName ?? string.Empty).ToLower().Contains(searchTerm) ||
                    (d.Specialization ?? string.Empty).ToLower().Contains(searchTerm)).ToList();
            }
            
            dataGridView.DataSource = new BindingList<DoctorViewModel>(
                doctors.Select(d => new DoctorViewModel
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Specialization = d.Specialization,
                    ContactNumber = d.ContactNumber,
                    Email = d.Email
                }).ToList());
        }
        
        protected override void Add()
        {
            using (var doctorForm = new DoctorForm())
            {
                if (doctorForm.ShowDialog() == DialogResult.OK)
                {
                    LoadDoctors();
                }
            }
        }
        
        protected override void Edit()
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView.SelectedRows[0].Cells["Id"].Value;
                var doctor = DataService.Instance.GetDoctorById(id);
                
                if (doctor != null)
                {
                    using (var doctorForm = new DoctorForm(doctor))
                    {
                        if (doctorForm.ShowDialog() == DialogResult.OK)
                        {
                            LoadDoctors();
                        }
                    }
                }
            }
            else
            {
                ShowSelectionRequired("doctor");
            }
        }
        
        protected override void Delete()
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView.SelectedRows[0];
                int doctorId = (int)selectedRow.Cells["Id"].Value;
                string doctorName = (string)selectedRow.Cells["FullName"].Value;
                
                // Check if doctor has appointments
                var doctorAppointments = DataService.Instance.GetAppointmentsByDoctor(doctorId);
                if (doctorAppointments.Count > 0)
                {
                    ShowCannotDelete(doctorName, "doctor", 
                        $"they have {doctorAppointments.Count} appointments scheduled. " +
                        $"Please cancel or reassign these appointments first.");
                    return;
                }
                
                var result = ConfirmDelete(doctorName, "doctor");
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        DataService.Instance.DeleteDoctor(doctorId);
                        LoadDoctors();
                    }
                    catch (Exception ex)
                    {
                        ShowError("deleting doctor", ex);
                    }
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
