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
    public class AppointmentManagementControl : UserControl
    {
        private DataGridView dgvAppointments;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Label lblTitle;
        private DateTimePicker dtpFilter;
        private Button btnFilter;
        private Button btnClearFilter;
        
        public AppointmentManagementControl()
        {
            InitializeComponent();
            LoadAppointments();
        }
        
        private void InitializeComponent()
        {
            // Setup control
            this.Size = new Size(940, 400);
            
            // Create title label
            lblTitle = new Label();
            lblTitle.Text = "Appointment Management";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Size = new Size(300, 30);
            
            // Create filter controls
            dtpFilter = new DateTimePicker();
            dtpFilter.Location = new Point(470, 10);
            dtpFilter.Size = new Size(200, 25);
            dtpFilter.Format = DateTimePickerFormat.Short;
            
            btnFilter = new Button();
            btnFilter.Text = "Filter by Date";
            btnFilter.Location = new Point(680, 10);
            btnFilter.Size = new Size(100, 25);
            btnFilter.Click += (s, e) => FilterAppointments();
            
            btnClearFilter = new Button();
            btnClearFilter.Text = "Clear";
            btnClearFilter.Location = new Point(790, 10);
            btnClearFilter.Size = new Size(60, 25);
            btnClearFilter.Click += (s, e) => LoadAppointments();
            
            // Create DataGridView
            dgvAppointments = new DataGridView();
            dgvAppointments.Location = new Point(10, 50);
            dgvAppointments.Size = new Size(810, 280);
            dgvAppointments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAppointments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAppointments.MultiSelect = false;
            dgvAppointments.ReadOnly = true;
            dgvAppointments.AllowUserToAddRows = false;
            dgvAppointments.AllowUserToDeleteRows = false;
            dgvAppointments.AllowUserToResizeRows = false;
            dgvAppointments.RowHeadersVisible = false;
            dgvAppointments.BackgroundColor = Color.White;
            
            // Create buttons
            btnAdd = new Button();
            btnAdd.Text = "Add Appointment";
            btnAdd.Location = new Point(830, 50);
            btnAdd.Size = new Size(100, 30);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Click += (s, e) => AddAppointment();
            
            btnEdit = new Button();
            btnEdit.Text = "Edit";
            btnEdit.Location = new Point(830, 90);
            btnEdit.Size = new Size(100, 30);
            btnEdit.BackColor = Color.LightBlue;
            btnEdit.Click += (s, e) => EditAppointment();
            
            btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.Location = new Point(830, 130);
            btnDelete.Size = new Size(100, 30);
            btnDelete.BackColor = Color.LightPink;
            btnDelete.Click += (s, e) => DeleteAppointment();
            
            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(dtpFilter);
            this.Controls.Add(btnFilter);
            this.Controls.Add(btnClearFilter);
            this.Controls.Add(dgvAppointments);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
        }
        
        private void LoadAppointments()
        {
            var appointments = DataService.Instance.GetAllAppointments();
            var doctors = DataService.Instance.GetAllDoctors();
            var patients = DataService.Instance.GetAllPatients();
            
            dgvAppointments.DataSource = null;
            dgvAppointments.Columns.Clear();
            
            var bindingList = new BindingList<AppointmentViewModel>(
                appointments.Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    PatientName = patients.FirstOrDefault(p => p.Id == a.PatientId)?.FullName ?? "Unknown",
                    DoctorName = doctors.FirstOrDefault(d => d.Id == a.DoctorId)?.FullName ?? "Unknown",
                    AppointmentDate = a.AppointmentDate,
                    Purpose = a.Purpose,
                    Status = a.Status
                }).OrderBy(a => a.AppointmentDate).ToList());
            
            dgvAppointments.DataSource = bindingList;
            
            if (dgvAppointments.Columns.Count > 0)
            {
                dgvAppointments.Columns["Id"].Visible = false;
                
                if (dgvAppointments.Columns.Contains("AppointmentDate"))
                {
                    dgvAppointments.Columns["AppointmentDate"].DefaultCellStyle.Format = "MM/dd/yyyy HH:mm";
                }
            }
        }
        
        private void FilterAppointments()
        {
            DateTime filterDate = dtpFilter.Value.Date;
            var appointments = DataService.Instance.GetAllAppointments()
                .Where(a => a.AppointmentDate.Date == filterDate)
                .ToList();
                
            var doctors = DataService.Instance.GetAllDoctors();
            var patients = DataService.Instance.GetAllPatients();
            
            dgvAppointments.DataSource = new BindingList<AppointmentViewModel>(
                appointments.Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    PatientName = patients.FirstOrDefault(p => p.Id == a.PatientId)?.FullName ?? "Unknown",
                    DoctorName = doctors.FirstOrDefault(d => d.Id == a.DoctorId)?.FullName ?? "Unknown",
                    AppointmentDate = a.AppointmentDate,
                    Purpose = a.Purpose,
                    Status = a.Status
                }).OrderBy(a => a.AppointmentDate).ToList());
        }
        
        private void AddAppointment()
        {
            using (var form = new AppointmentForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadAppointments();
                }
            }
        }
        
        private void EditAppointment()
        {
            if (dgvAppointments.SelectedRows.Count > 0)
            {
                var selectedRow = dgvAppointments.SelectedRows[0];
                int appointmentId = (int)selectedRow.Cells["Id"].Value;
                
                var appointment = DataService.Instance.GetAppointmentById(appointmentId);
                if (appointment != null)
                {
                    using (var form = new AppointmentForm(appointment))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            LoadAppointments();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an appointment to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void DeleteAppointment()
        {
            if (dgvAppointments.SelectedRows.Count > 0)
            {
                var selectedRow = dgvAppointments.SelectedRows[0];
                int appointmentId = (int)selectedRow.Cells["Id"].Value;
                
                var result = MessageBox.Show("Are you sure you want to delete this appointment?", 
                    "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    DataService.Instance.DeleteAppointment(appointmentId);
                    LoadAppointments();
                }
            }
            else
            {
                MessageBox.Show("Please select an appointment to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
    
    // View model for the DataGridView
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
