using SimpleHospitalApp.Models;
using SimpleHospitalApp.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleHospitalApp
{
    public class AppointmentForm : Form
    {
        private Label lblTitle;
        private Label lblPatient;
        private ComboBox cboPatient;
        private Label lblDoctor;
        private ComboBox cboDoctor;
        private Label lblDate;
        private DateTimePicker dtpDate;
        private Label lblTime;
        private DateTimePicker dtpTime;
        private Label lblPurpose;
        private TextBox txtPurpose;
        private Label lblStatus;
        private ComboBox cboStatus;
        private Label lblNotes;
        private TextBox txtNotes;
        private Button btnSave;
        private Button btnCancel;
        
        private Appointment? _appointment;
        
        public AppointmentForm(Appointment? appointment = null)
        {
            _appointment = appointment;
            InitializeComponent();
            LoadDoctorsAndPatients();
            
            if (_appointment != null)
            {
                LoadAppointmentData();
                lblTitle.Text = "Edit Appointment";
            }
        }
        
        private void InitializeComponent()
        {
            // Form properties
            this.Text = "Appointment Information";
            this.Size = new Size(500, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
            
            // Title
            lblTitle = new Label();
            lblTitle.Text = "Schedule New Appointment";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(300, 30);
            
            // Patient
            lblPatient = new Label();
            lblPatient.Text = "Patient:";
            lblPatient.Location = new Point(20, 70);
            lblPatient.Size = new Size(100, 20);
            
            cboPatient = new ComboBox();
            cboPatient.Location = new Point(150, 70);
            cboPatient.Size = new Size(300, 20);
            cboPatient.DropDownStyle = ComboBoxStyle.DropDownList;
            
            // Doctor
            lblDoctor = new Label();
            lblDoctor.Text = "Doctor:";
            lblDoctor.Location = new Point(20, 100);
            lblDoctor.Size = new Size(100, 20);
            
            cboDoctor = new ComboBox();
            cboDoctor.Location = new Point(150, 100);
            cboDoctor.Size = new Size(300, 20);
            cboDoctor.DropDownStyle = ComboBoxStyle.DropDownList;
            
            // Date
            lblDate = new Label();
            lblDate.Text = "Date:";
            lblDate.Location = new Point(20, 130);
            lblDate.Size = new Size(100, 20);
            
            dtpDate = new DateTimePicker();
            dtpDate.Location = new Point(150, 130);
            dtpDate.Size = new Size(300, 20);
            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.Value = DateTime.Now.AddDays(1);
            dtpDate.MinDate = DateTime.Now;
            
            // Time
            lblTime = new Label();
            lblTime.Text = "Time:";
            lblTime.Location = new Point(20, 160);
            lblTime.Size = new Size(100, 20);
            
            dtpTime = new DateTimePicker();
            dtpTime.Location = new Point(150, 160);
            dtpTime.Size = new Size(300, 20);
            dtpTime.Format = DateTimePickerFormat.Time;
            dtpTime.ShowUpDown = true;
            dtpTime.Value = DateTime.Today.AddHours(9);
            
            // Purpose
            lblPurpose = new Label();
            lblPurpose.Text = "Purpose:";
            lblPurpose.Location = new Point(20, 190);
            lblPurpose.Size = new Size(100, 20);
            
            txtPurpose = new TextBox();
            txtPurpose.Location = new Point(150, 190);
            txtPurpose.Size = new Size(300, 20);
            
            // Status
            lblStatus = new Label();
            lblStatus.Text = "Status:";
            lblStatus.Location = new Point(20, 220);
            lblStatus.Size = new Size(100, 20);
            
            cboStatus = new ComboBox();
            cboStatus.Location = new Point(150, 220);
            cboStatus.Size = new Size(300, 20);
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.Items.AddRange(new string[] { "Scheduled", "Completed", "Canceled" });
            cboStatus.SelectedIndex = 0;
            
            // Notes
            lblNotes = new Label();
            lblNotes.Text = "Notes:";
            lblNotes.Location = new Point(20, 250);
            lblNotes.Size = new Size(100, 20);
            
            txtNotes = new TextBox();
            txtNotes.Location = new Point(150, 250);
            txtNotes.Size = new Size(300, 80);
            txtNotes.Multiline = true;
            
            // Buttons
            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Location = new Point(150, 350);
            btnSave.Size = new Size(100, 30);
            btnSave.BackColor = Color.LightGreen;
            btnSave.Click += (s, e) => SaveAppointment();
            
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(260, 350);
            btnCancel.Size = new Size(100, 30);
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            
            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblPatient);
            this.Controls.Add(cboPatient);
            this.Controls.Add(lblDoctor);
            this.Controls.Add(cboDoctor);
            this.Controls.Add(lblDate);
            this.Controls.Add(dtpDate);
            this.Controls.Add(lblTime);
            this.Controls.Add(dtpTime);
            this.Controls.Add(lblPurpose);
            this.Controls.Add(txtPurpose);
            this.Controls.Add(lblStatus);
            this.Controls.Add(cboStatus);
            this.Controls.Add(lblNotes);
            this.Controls.Add(txtNotes);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }
        
        private void LoadDoctorsAndPatients()
        {
            // Load patients
            var patients = DataService.Instance.GetAllPatients();
            cboPatient.DisplayMember = "FullName";
            cboPatient.ValueMember = "Id";
            cboPatient.DataSource = patients;
            
            // Load doctors
            var doctors = DataService.Instance.GetAllDoctors();
            cboDoctor.DisplayMember = "FullName";
            cboDoctor.ValueMember = "Id";
            cboDoctor.DataSource = doctors;
        }
        
        private void LoadAppointmentData()
        {
            if (_appointment == null) return;
            
            // Set patient
            for (int i = 0; i < cboPatient.Items.Count; i++)
            {
                var patient = (Patient)cboPatient.Items[i];
                if (patient.Id == _appointment.PatientId)
                {
                    cboPatient.SelectedIndex = i;
                    break;
                }
            }
            
            // Set doctor
            for (int i = 0; i < cboDoctor.Items.Count; i++)
            {
                var doctor = (Doctor)cboDoctor.Items[i];
                if (doctor.Id == _appointment.DoctorId)
                {
                    cboDoctor.SelectedIndex = i;
                    break;
                }
            }
            
            // Set date and time
            dtpDate.Value = _appointment.AppointmentDate.Date;
            dtpTime.Value = DateTime.Today.Add(_appointment.AppointmentDate.TimeOfDay);
            
            // Set other fields
            txtPurpose.Text = _appointment.Purpose;
            cboStatus.SelectedItem = _appointment.Status;
            txtNotes.Text = _appointment.Notes;
        }
        
        private void SaveAppointment()
        {
            // Validate inputs
            if (cboPatient.SelectedItem == null || cboDoctor.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtPurpose.Text))
            {
                MessageBox.Show("Patient, doctor, and purpose are required.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Create or update appointment
            if (_appointment == null)
            {
                _appointment = new Appointment();
            }
            
            _appointment.PatientId = cboPatient.SelectedItem != null ? ((Patient)cboPatient.SelectedItem).Id : 0;
            _appointment.DoctorId = cboDoctor.SelectedItem != null ? ((Doctor)cboDoctor.SelectedItem).Id : 0;
            
            // Combine date and time
            DateTime dateValue = dtpDate.Value.Date;
            DateTime timeValue = dtpTime.Value;
            _appointment.AppointmentDate = dateValue.Add(timeValue.TimeOfDay);
            
            _appointment.Purpose = txtPurpose.Text.Trim();
            _appointment.Status = cboStatus.SelectedItem?.ToString() ?? "Scheduled";
            _appointment.Notes = txtNotes.Text.Trim();
            
            // Save to data service
            if (_appointment.Id == 0)
            {
                DataService.Instance.AddAppointment(_appointment);
            }
            else
            {
                DataService.Instance.UpdateAppointment(_appointment);
            }
            
            this.DialogResult = DialogResult.OK;
        }
    }
}
