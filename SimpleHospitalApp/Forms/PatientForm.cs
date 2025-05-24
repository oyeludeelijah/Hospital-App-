using SimpleHospitalApp.Models;
using SimpleHospitalApp.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleHospitalApp
{
    public class PatientForm : Form
    {
        private Label lblTitle;
        private Label lblFirstName;
        private TextBox txtFirstName;
        private Label lblLastName;
        private TextBox txtLastName;
        private Label lblDateOfBirth;
        private DateTimePicker dtpDateOfBirth;
        private Label lblGender;
        private ComboBox cboGender;
        private Label lblContactNumber;
        private TextBox txtContactNumber;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblAddress;
        private TextBox txtAddress;
        private Label lblMedicalHistory;
        private TextBox txtMedicalHistory;
        private Button btnSave;
        private Button btnCancel;
        
        private Patient? _patient;
        
        public PatientForm(Patient? patient = null)
        {
            _patient = patient;
            InitializeComponent();
            
            if (_patient != null)
            {
                LoadPatientData();
                lblTitle.Text = "Edit Patient";
            }
        }
        
        private void InitializeComponent()
        {
            // Form properties
            this.Text = "Patient Information";
            this.Size = new Size(500, 550);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
            
            // Title
            lblTitle = new Label();
            lblTitle.Text = "Add New Patient";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(200, 30);
            
            // First Name
            lblFirstName = new Label();
            lblFirstName.Text = "First Name:";
            lblFirstName.Location = new Point(20, 70);
            lblFirstName.Size = new Size(100, 20);
            
            txtFirstName = new TextBox();
            txtFirstName.Location = new Point(150, 70);
            txtFirstName.Size = new Size(300, 20);
            
            // Last Name
            lblLastName = new Label();
            lblLastName.Text = "Last Name:";
            lblLastName.Location = new Point(20, 100);
            lblLastName.Size = new Size(100, 20);
            
            txtLastName = new TextBox();
            txtLastName.Location = new Point(150, 100);
            txtLastName.Size = new Size(300, 20);
            
            // Date of Birth
            lblDateOfBirth = new Label();
            lblDateOfBirth.Text = "Date of Birth:";
            lblDateOfBirth.Location = new Point(20, 130);
            lblDateOfBirth.Size = new Size(100, 20);
            
            dtpDateOfBirth = new DateTimePicker();
            dtpDateOfBirth.Location = new Point(150, 130);
            dtpDateOfBirth.Size = new Size(300, 20);
            dtpDateOfBirth.Format = DateTimePickerFormat.Short;
            dtpDateOfBirth.Value = DateTime.Now.AddYears(-30);
            
            // Gender
            lblGender = new Label();
            lblGender.Text = "Gender:";
            lblGender.Location = new Point(20, 160);
            lblGender.Size = new Size(100, 20);
            
            cboGender = new ComboBox();
            cboGender.Location = new Point(150, 160);
            cboGender.Size = new Size(300, 20);
            cboGender.DropDownStyle = ComboBoxStyle.DropDownList;
            cboGender.Items.AddRange(new string[] { "Male", "Female", "Other" });
            cboGender.SelectedIndex = 0;
            
            // Contact Number
            lblContactNumber = new Label();
            lblContactNumber.Text = "Contact Number:";
            lblContactNumber.Location = new Point(20, 190);
            lblContactNumber.Size = new Size(100, 20);
            
            txtContactNumber = new TextBox();
            txtContactNumber.Location = new Point(150, 190);
            txtContactNumber.Size = new Size(300, 20);
            
            // Email
            lblEmail = new Label();
            lblEmail.Text = "Email:";
            lblEmail.Location = new Point(20, 220);
            lblEmail.Size = new Size(100, 20);
            
            txtEmail = new TextBox();
            txtEmail.Location = new Point(150, 220);
            txtEmail.Size = new Size(300, 20);
            
            // Address
            lblAddress = new Label();
            lblAddress.Text = "Address:";
            lblAddress.Location = new Point(20, 250);
            lblAddress.Size = new Size(100, 20);
            
            txtAddress = new TextBox();
            txtAddress.Location = new Point(150, 250);
            txtAddress.Size = new Size(300, 60);
            txtAddress.Multiline = true;
            
            // Medical History
            lblMedicalHistory = new Label();
            lblMedicalHistory.Text = "Medical History:";
            lblMedicalHistory.Location = new Point(20, 320);
            lblMedicalHistory.Size = new Size(100, 20);
            
            txtMedicalHistory = new TextBox();
            txtMedicalHistory.Location = new Point(150, 320);
            txtMedicalHistory.Size = new Size(300, 100);
            txtMedicalHistory.Multiline = true;
            
            // Buttons
            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Location = new Point(150, 440);
            btnSave.Size = new Size(100, 30);
            btnSave.BackColor = Color.LightGreen;
            btnSave.Click += (s, e) => SavePatient();
            
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(260, 440);
            btnCancel.Size = new Size(100, 30);
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            
            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblFirstName);
            this.Controls.Add(txtFirstName);
            this.Controls.Add(lblLastName);
            this.Controls.Add(txtLastName);
            this.Controls.Add(lblDateOfBirth);
            this.Controls.Add(dtpDateOfBirth);
            this.Controls.Add(lblGender);
            this.Controls.Add(cboGender);
            this.Controls.Add(lblContactNumber);
            this.Controls.Add(txtContactNumber);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblAddress);
            this.Controls.Add(txtAddress);
            this.Controls.Add(lblMedicalHistory);
            this.Controls.Add(txtMedicalHistory);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }
        
        private void LoadPatientData()
        {
            if (_patient == null) return;
            
            txtFirstName.Text = _patient.FirstName;
            txtLastName.Text = _patient.LastName;
            dtpDateOfBirth.Value = _patient.DateOfBirth;
            cboGender.SelectedItem = _patient.Gender;
            txtContactNumber.Text = _patient.ContactNumber;
            txtEmail.Text = _patient.Email;
            txtAddress.Text = _patient.Address;
            txtMedicalHistory.Text = _patient.MedicalHistory;
        }
        
        private void SavePatient()
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("First name and last name are required.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Create or update patient
            if (_patient == null)
            {
                _patient = new Patient();
            }
            
            _patient.FirstName = txtFirstName.Text.Trim();
            _patient.LastName = txtLastName.Text.Trim();
            _patient.DateOfBirth = dtpDateOfBirth.Value;
            _patient.Gender = cboGender.SelectedItem.ToString() ?? "Unknown";
            _patient.ContactNumber = txtContactNumber.Text.Trim();
            _patient.Email = txtEmail.Text.Trim();
            _patient.Address = txtAddress.Text.Trim();
            _patient.MedicalHistory = txtMedicalHistory.Text.Trim();
            
            // Save to data service
            if (_patient.Id == 0)
            {
                DataService.Instance.AddPatient(_patient);
            }
            else
            {
                DataService.Instance.UpdatePatient(_patient);
            }
            
            this.DialogResult = DialogResult.OK;
        }
    }
}
