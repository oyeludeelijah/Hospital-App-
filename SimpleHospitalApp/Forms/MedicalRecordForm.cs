using SimpleHospitalApp.Data;
using SimpleHospitalApp.Models;
using SimpleHospitalApp.Helpers;
using SimpleHospitalApp.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleHospitalApp.Forms
{
    public class MedicalRecordForm : Form
    {
        private Label lblTitle;
        private Label lblPatient;
        private Label lblDiagnosis;
        private Label lblTreatment;
        private Label lblPrescription;
        private Label lblNotes;
        private ComboBox cmbPatient;
        private TextBox txtDiagnosis;
        private TextBox txtTreatment;
        private TextBox txtPrescription;
        private TextBox txtNotes;
        private Button btnSave;
        private Button btnCancel;
        
        private MedicalRecord? _record;
        private bool _isEdit;
        
        public MedicalRecordForm(MedicalRecord? record = null)
        {
            _record = record;
            _isEdit = record != null;
            
            InitializeComponent();
            LoadPatients();
            
            if (_isEdit)
            {
                LoadRecordData();
            }
        }
        
        private void InitializeComponent()
        {
            // Form settings
            this.Text = _isEdit ? "Edit Medical Record" : "Add Medical Record";
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            // Create title
            lblTitle = new Label();
            lblTitle.Text = _isEdit ? "Edit Medical Record" : "Add Medical Record";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Size = new Size(450, 30);
            lblTitle.Location = new Point(20, 20);
            
            // Create patient label and combobox
            lblPatient = new Label();
            lblPatient.Text = "Patient:";
            lblPatient.Size = new Size(100, 25);
            lblPatient.Location = new Point(20, 70);
            
            cmbPatient = new ComboBox();
            cmbPatient.Size = new Size(300, 25);
            cmbPatient.Location = new Point(130, 70);
            cmbPatient.DropDownStyle = ComboBoxStyle.DropDownList;
            
            // Create diagnosis label and textbox
            lblDiagnosis = new Label();
            lblDiagnosis.Text = "Diagnosis:";
            lblDiagnosis.Size = new Size(100, 25);
            lblDiagnosis.Location = new Point(20, 110);
            
            txtDiagnosis = new TextBox();
            txtDiagnosis.Size = new Size(300, 50);
            txtDiagnosis.Location = new Point(130, 110);
            
            // Create treatment label and textbox
            lblTreatment = new Label();
            lblTreatment.Text = "Treatment:";
            lblTreatment.Size = new Size(100, 25);
            lblTreatment.Location = new Point(20, 150);
            
            txtTreatment = new TextBox();
            txtTreatment.Multiline = true;
            txtTreatment.Size = new Size(300, 60);
            txtTreatment.Location = new Point(130, 150);
            
            // Create prescription label and textbox
            lblPrescription = new Label();
            lblPrescription.Text = "Prescription:";
            lblPrescription.Size = new Size(100, 25);
            lblPrescription.Location = new Point(20, 230);
            
            txtPrescription = new TextBox();
            txtPrescription.Multiline = true;
            txtPrescription.Size = new Size(300, 60);
            txtPrescription.Location = new Point(130, 230);
            
            // Create notes label and textbox
            lblNotes = new Label();
            lblNotes.Text = "Notes:";
            lblNotes.Size = new Size(100, 25);
            lblNotes.Location = new Point(20, 310);
            
            txtNotes = new TextBox();
            txtNotes.Multiline = true;
            txtNotes.Size = new Size(300, 80);
            txtNotes.Location = new Point(130, 310);
            
            // Create buttons
            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Size = new Size(100, 35);
            btnSave.Location = new Point(130, 410);
            btnSave.BackColor = SystemColors.ButtonFace;
            btnSave.Click += BtnSave_Click;
            
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(100, 35);
            btnCancel.Location = new Point(240, 410);
            btnCancel.BackColor = SystemColors.ButtonFace;
            btnCancel.Click += (s, e) => this.Close();
            
            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblPatient);
            this.Controls.Add(cmbPatient);
            this.Controls.Add(lblDiagnosis);
            this.Controls.Add(txtDiagnosis);
            this.Controls.Add(lblTreatment);
            this.Controls.Add(txtTreatment);
            this.Controls.Add(lblPrescription);
            this.Controls.Add(txtPrescription);
            this.Controls.Add(lblNotes);
            this.Controls.Add(txtNotes);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }
        
        private void LoadPatients()
        {
            try
            {
                // Get patients from DataService
                var patients = DataService.Instance.GetAllPatients();
                
                // Setup the combo box
                cmbPatient.DisplayMember = "Text";
                cmbPatient.ValueMember = "Value";
                
                // Add patients to combo box
                var items = new List<ComboBoxItem>();
                foreach (var patient in patients)
                {
                    items.Add(new ComboBoxItem { 
                        Text = $"{patient.FirstName} {patient.LastName}", 
                        Value = patient.Id 
                    });
                }
                
                cmbPatient.DataSource = items;
                
                if (cmbPatient.Items.Count > 0)
                {
                    cmbPatient.SelectedIndex = 0;
                }
                
                // If no patients, inform the user
                if (patients.Count == 0)
                {
                    MessageBox.Show("No patients found. Please add patients first.", 
                        "No Patients", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading patients: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LoadRecordData()
        {
            if (_record == null) return;
            
            try
            {
                // Get patient from DataService
                var patient = DataService.Instance.GetPatientById(_record.PatientId);
                
                if (patient != null)
                {
                    // Find and select the patient in the combobox
                    for (int i = 0; i < cmbPatient.Items.Count; i++)
                    {
                        if (cmbPatient.Items[i] is ComboBoxItem item && 
                            item.Value != null && 
                            (int)item.Value == patient.Id)
                        {
                            cmbPatient.SelectedIndex = i;
                            break;
                        }
                    }
                }
                
                // Set form field values
                txtDiagnosis.Text = _record?.Diagnosis ?? string.Empty;
                txtTreatment.Text = _record?.Treatment ?? string.Empty;
                txtPrescription.Text = _record?.Prescription ?? string.Empty;
                txtNotes.Text = _record?.Notes ?? string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading record data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDiagnosis.Text))
            {
                MessageBox.Show("Please enter a diagnosis.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Validate patient selection
            int patientId = GetSelectedPatientId();
            if (patientId <= 0)
            {
                MessageBox.Show("Please select a valid patient.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                // Use DataService for data operations
                if (_isEdit && _record != null)
                {
                    // Update existing record
                    _record.PatientId = patientId;
                    _record.Diagnosis = txtDiagnosis.Text.Trim();
                    _record.Treatment = txtTreatment.Text.Trim();
                    _record.Prescription = txtPrescription.Text.Trim();
                    _record.Notes = txtNotes.Text.Trim();
                    
                    // Update in DataService
                    DataService.Instance.UpdateMedicalRecord(_record);
                    MessageBox.Show("Medical record updated successfully.", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Create new record
                    var newRecord = new MedicalRecord
                    {
                        PatientId = patientId,
                        RecordDate = DateTime.Now,
                        Diagnosis = txtDiagnosis.Text.Trim(),
                        Treatment = txtTreatment.Text.Trim(),
                        Prescription = txtPrescription.Text.Trim(),
                        Notes = txtNotes.Text.Trim()
                    };
                    
                    // Add to DataService
                    DataService.Instance.AddMedicalRecord(newRecord);
                    MessageBox.Show("Medical record added successfully.", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving medical record: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private int GetSelectedPatientId()
        {
            if (cmbPatient.SelectedItem is ComboBoxItem item && item.Value != null)
            {
                return (int)item.Value;
            }
            return 0;
        }
    }
}
