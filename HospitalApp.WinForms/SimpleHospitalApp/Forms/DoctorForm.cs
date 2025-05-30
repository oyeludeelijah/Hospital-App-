using SimpleHospitalApp.Models;
using SimpleHospitalApp.Services;
using SimpleHospitalApp.Helpers;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleHospitalApp
{
    public class DoctorForm : Form
    {
        private Label lblTitle;
        private Label lblFirstName;
        private TextBox txtFirstName;
        private Label lblLastName;
        private TextBox txtLastName;
        private Label lblSpecialization;
        private ComboBox cboSpecialization;
        private Label lblDepartment;
        private ComboBox cboDepartment;
        private Label lblContactNumber;
        private TextBox txtContactNumber;
        private Label lblEmail;
        private TextBox txtEmail;
        private Button btnSave;
        private Button btnCancel;
        
        private Doctor? _doctor;
        
        public DoctorForm(Doctor? doctor = null)
        {
            _doctor = doctor;
            InitializeComponent();
            LoadDepartments();
            
            if (_doctor != null)
            {
                LoadDoctorData();
                lblTitle.Text = "Edit Doctor";
            }
        }
        
        private void InitializeComponent()
        {
            // Form properties
            this.Text = "Doctor Information";
            this.Size = new Size(500, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
            
            // Title
            lblTitle = new Label();
            lblTitle.Text = "Add New Doctor";
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
            
            // Specialization
            lblSpecialization = new Label();
            lblSpecialization.Text = "Specialization:";
            lblSpecialization.Location = new Point(20, 130);
            lblSpecialization.Size = new Size(100, 20);
            
            cboSpecialization = new ComboBox();
            cboSpecialization.Location = new Point(150, 130);
            cboSpecialization.Size = new Size(300, 20);
            cboSpecialization.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSpecialization.Items.AddRange(new string[] { 
                "Cardiology", "Dermatology", "Endocrinology", "Gastroenterology", 
                "Hematology", "Neurology", "Obstetrics", "Oncology", "Ophthalmology", 
                "Orthopedics", "Pediatrics", "Psychiatry", "Radiology", "Urology" 
            });
            
            // Department
            lblDepartment = new Label();
            lblDepartment.Text = "Department:";
            lblDepartment.Location = new Point(20, 160);
            lblDepartment.Size = new Size(100, 20);
            
            cboDepartment = new ComboBox();
            cboDepartment.Location = new Point(150, 160);
            cboDepartment.Size = new Size(300, 20);
            cboDepartment.DropDownStyle = ComboBoxStyle.DropDownList;
            
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
            
            // Buttons
            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Location = new Point(150, 270);
            btnSave.Size = new Size(100, 30);
            btnSave.BackColor = Color.LightGreen;
            btnSave.Click += (s, e) => SaveDoctor();
            
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(260, 270);
            btnCancel.Size = new Size(100, 30);
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            
            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblFirstName);
            this.Controls.Add(txtFirstName);
            this.Controls.Add(lblLastName);
            this.Controls.Add(txtLastName);
            this.Controls.Add(lblSpecialization);
            this.Controls.Add(cboSpecialization);
            this.Controls.Add(lblDepartment);
            this.Controls.Add(cboDepartment);
            this.Controls.Add(lblContactNumber);
            this.Controls.Add(txtContactNumber);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }
        
        private void LoadDoctorData()
        {
            if (_doctor == null) return;
            
            txtFirstName.Text = _doctor.FirstName;
            txtLastName.Text = _doctor.LastName;
            cboSpecialization.SelectedItem = _doctor.Specialization;
            txtContactNumber.Text = _doctor.ContactNumber;
            txtEmail.Text = _doctor.Email;
            
            // Set selected department
            if (_doctor.DepartmentId.HasValue)
            {
                for (int i = 0; i < cboDepartment.Items.Count; i++)
                {
                    var item = (ComboBoxItem)cboDepartment.Items[i];
                    if ((int)item.Value == _doctor.DepartmentId.Value)
                    {
                        cboDepartment.SelectedIndex = i;
                        break;
                    }
                }
            }
        }
        
        private void LoadDepartments()
        {
            try
            {
                // Get departments from DataService
                var departments = DataService.Instance.GetAllDepartments();
                
                // Setup the combo box
                cboDepartment.DisplayMember = "Text";
                cboDepartment.ValueMember = "Value";
                
                // Add a "None" option first
                var items = new List<ComboBoxItem> { new ComboBoxItem { Text = "-- Select Department --", Value = 0 } };
                
                // Add departments to combo box
                foreach (var department in departments)
                {
                    items.Add(new ComboBoxItem { 
                        Text = department.Name, 
                        Value = department.Id 
                    });
                }
                
                cboDepartment.DataSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading departments: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void SaveDoctor()
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                cboSpecialization.SelectedItem == null)
            {
                MessageBox.Show("First name, last name, and specialization are required.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Create or update doctor
            if (_doctor == null)
            {
                _doctor = new Doctor();
            }
            
            _doctor.FirstName = txtFirstName.Text.Trim();
            _doctor.LastName = txtLastName.Text.Trim();
            _doctor.Specialization = cboSpecialization.SelectedItem?.ToString() ?? "General";
            _doctor.ContactNumber = txtContactNumber.Text.Trim();
            _doctor.Email = txtEmail.Text.Trim();
            
            // Set department
            if (cboDepartment.SelectedItem != null)
            {
                var selectedDepartment = (ComboBoxItem)cboDepartment.SelectedItem;
                _doctor.DepartmentId = (int)selectedDepartment.Value;
            }
            else
            {
                _doctor.DepartmentId = null;
            }
            
            // Save to data service
            if (_doctor.Id == 0)
            {
                DataService.Instance.AddDoctor(_doctor);
            }
            else
            {
                DataService.Instance.UpdateDoctor(_doctor);
            }
            
            this.DialogResult = DialogResult.OK;
        }
    }
}
