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
    public class MedicalRecordManagementControl : UserControl
    {
        private DataGridView dgvMedicalRecords;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnView;
        private Label lblTitle;
        private ComboBox cmbPatient;
        private Button btnFilter;
        private Button btnClearFilter;
        
        public MedicalRecordManagementControl()
        {
            InitializeComponent();
            LoadPatients();
            LoadMedicalRecords();
        }
        
        private void InitializeComponent()
        {
            // Setup control
            this.Size = new Size(940, 400);
            
            // Create title label
            lblTitle = new Label();
            lblTitle.Text = "Medical Record Management";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Size = new Size(300, 30);
            
            // Create filter controls
            Label lblPatient = new Label();
            lblPatient.Text = "Filter by Patient:";
            lblPatient.Location = new Point(400, 10);
            lblPatient.Size = new Size(100, 25);
            lblPatient.TextAlign = ContentAlignment.MiddleRight;
            
            cmbPatient = new ComboBox();
            cmbPatient.Location = new Point(500, 10);
            cmbPatient.Size = new Size(150, 25);
            cmbPatient.DropDownStyle = ComboBoxStyle.DropDownList;
            
            btnFilter = new Button();
            btnFilter.Text = "Filter";
            btnFilter.Location = new Point(660, 10);
            btnFilter.Size = new Size(60, 25);
            btnFilter.Click += (s, e) => FilterMedicalRecords();
            
            btnClearFilter = new Button();
            btnClearFilter.Text = "Clear";
            btnClearFilter.Location = new Point(730, 10);
            btnClearFilter.Size = new Size(60, 25);
            btnClearFilter.Click += (s, e) => LoadMedicalRecords();
            
            // Create DataGridView
            dgvMedicalRecords = new DataGridView();
            dgvMedicalRecords.Location = new Point(10, 50);
            dgvMedicalRecords.Size = new Size(810, 280);
            dgvMedicalRecords.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMedicalRecords.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMedicalRecords.MultiSelect = false;
            dgvMedicalRecords.ReadOnly = true;
            dgvMedicalRecords.AllowUserToAddRows = false;
            dgvMedicalRecords.AllowUserToDeleteRows = false;
            dgvMedicalRecords.AllowUserToResizeRows = false;
            dgvMedicalRecords.RowHeadersVisible = false;
            dgvMedicalRecords.BackgroundColor = Color.White;
            
            // Create buttons
            btnAdd = new Button();
            btnAdd.Text = "Add Record";
            btnAdd.Location = new Point(830, 50);
            btnAdd.Size = new Size(100, 30);
            btnAdd.Click += (s, e) => AddMedicalRecord();
            
            btnEdit = new Button();
            btnEdit.Text = "Edit";
            btnEdit.Location = new Point(830, 90);
            btnEdit.Size = new Size(100, 30);
            btnEdit.Click += (s, e) => EditMedicalRecord();
            
            btnView = new Button();
            btnView.Text = "View Details";
            btnView.Location = new Point(830, 130);
            btnView.Size = new Size(100, 30);
            btnView.Click += (s, e) => ViewMedicalRecord();
            
            btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.Location = new Point(830, 170);
            btnDelete.Size = new Size(100, 30);
            btnDelete.Click += (s, e) => DeleteMedicalRecord();
            
            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblPatient);
            this.Controls.Add(cmbPatient);
            this.Controls.Add(btnFilter);
            this.Controls.Add(btnClearFilter);
            this.Controls.Add(dgvMedicalRecords);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnView);
            this.Controls.Add(btnDelete);
        }
        
        private void LoadPatients()
        {
            try
            {
                var patients = DataService.Instance.GetAllPatients();
                
                cmbPatient.Items.Clear();
                cmbPatient.Items.Add("-- All Patients --");
                
                foreach (var patient in patients)
                {
                    cmbPatient.Items.Add(new PatientComboItem { 
                        Display = $"{patient.FirstName} {patient.LastName}", 
                        Id = patient.Id 
                    });
                }
                
                if (cmbPatient.Items.Count > 0)
                {
                    cmbPatient.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading patients: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LoadMedicalRecords()
        {
            try
            {
                var medicalRecords = DataService.Instance.GetAllMedicalRecords();
                var patients = DataService.Instance.GetAllPatients();
                
                var viewModels = new List<MedicalRecordViewModel>();
                
                foreach (var record in medicalRecords)
                {
                    var patient = patients.FirstOrDefault(p => p.Id == record.PatientId);
                    viewModels.Add(new MedicalRecordViewModel
                    {
                        Id = record.Id,
                        PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "Unknown",
                        RecordDate = record.RecordDate,
                        Diagnosis = record.Diagnosis,
                        Treatment = record.Treatment
                    });
                }
                
                dgvMedicalRecords.DataSource = new BindingList<MedicalRecordViewModel>(
                    viewModels.OrderByDescending(r => r.RecordDate).ToList());
                
                if (dgvMedicalRecords.Columns.Count > 0)
                {
                    dgvMedicalRecords.Columns["Id"].Visible = false;
                    
                    if (dgvMedicalRecords.Columns.Contains("RecordDate"))
                    {
                        dgvMedicalRecords.Columns["RecordDate"].DefaultCellStyle.Format = "MM/dd/yyyy";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading medical records: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void FilterMedicalRecords()
        {
            try
            {
                // Get all records
                var medicalRecords = DataService.Instance.GetAllMedicalRecords();
                var patients = DataService.Instance.GetAllPatients();
                
                // Apply patient filter if selected
                if (cmbPatient.SelectedIndex > 0 && cmbPatient.SelectedItem is PatientComboItem selectedPatient)
                {
                    medicalRecords = medicalRecords.Where(r => r.PatientId == selectedPatient.Id).ToList();
                }
                
                // Create view models
                var viewModels = new List<MedicalRecordViewModel>();
                
                foreach (var record in medicalRecords)
                {
                    var patient = patients.FirstOrDefault(p => p.Id == record.PatientId);
                    viewModels.Add(new MedicalRecordViewModel
                    {
                        Id = record.Id,
                        PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "Unknown",
                        RecordDate = record.RecordDate,
                        Diagnosis = record.Diagnosis,
                        Treatment = record.Treatment
                    });
                }
                
                // Update grid
                dgvMedicalRecords.DataSource = new BindingList<MedicalRecordViewModel>(
                    viewModels.OrderByDescending(r => r.RecordDate).ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering medical records: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void AddMedicalRecord()
        {
            var medicalRecordForm = new MedicalRecordForm();
            if (medicalRecordForm.ShowDialog() == DialogResult.OK)
            {
                LoadMedicalRecords();
            }
        }
        
        private void EditMedicalRecord()
        {
            if (dgvMedicalRecords.SelectedRows.Count > 0)
            {
                int recordId = (int)dgvMedicalRecords.SelectedRows[0].Cells["Id"].Value;
                var record = DataService.Instance.GetMedicalRecordById(recordId);
                
                if (record != null)
                {
                    var medicalRecordForm = new MedicalRecordForm(record);
                    if (medicalRecordForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadMedicalRecords();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a medical record to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void ViewMedicalRecord()
        {
            if (dgvMedicalRecords.SelectedRows.Count > 0)
            {
                int recordId = (int)dgvMedicalRecords.SelectedRows[0].Cells["Id"].Value;
                var record = DataService.Instance.GetMedicalRecordById(recordId);
                
                if (record != null)
                {
                    var patient = DataService.Instance.GetPatientById(record.PatientId);
                    string patientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "Unknown";
                    
                    var detailMessage = $"Medical Record Details\n\n" +
                        $"Patient: {patientName}\n" +
                        $"Date: {record.RecordDate:MM/dd/yyyy}\n" +
                        $"Diagnosis: {record.Diagnosis}\n" +
                        $"Treatment: {record.Treatment}\n" +
                        $"Prescription: {record.Prescription}\n\n" +
                        $"Notes: {record.Notes}";
                    
                    MessageBox.Show(detailMessage, "Medical Record Details", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a medical record to view.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void DeleteMedicalRecord()
        {
            if (dgvMedicalRecords.SelectedRows.Count > 0)
            {
                int recordId = (int)dgvMedicalRecords.SelectedRows[0].Cells["Id"].Value;
                string patientName = dgvMedicalRecords.SelectedRows[0].Cells["PatientName"].Value.ToString();
                
                var result = MessageBox.Show($"Are you sure you want to delete the medical record for {patientName}?", 
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    DataService.Instance.DeleteMedicalRecord(recordId);
                    MessageBox.Show("Medical record deleted successfully.", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMedicalRecords();
                }
            }
            else
            {
                MessageBox.Show("Please select a medical record to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
    
    public class MedicalRecordViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime RecordDate { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
    }
    
    public class PatientComboItem
    {
        public int Id { get; set; }
        public string Display { get; set; } = string.Empty;
        
        public override string ToString()
        {
            return Display;
        }
    }
}
