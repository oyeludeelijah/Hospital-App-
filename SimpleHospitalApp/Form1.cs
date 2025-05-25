using SimpleHospitalApp.Services;
using System.Drawing;

namespace SimpleHospitalApp;

public partial class Form1 : Form
{
    private Button btnPatients;
    private Button btnDoctors;
    private Button btnAppointments;
    private Label lblTitle;
    private Panel pnlContent;
    
    public Form1()
    {
        InitializeComponent();
        InitializeCustomComponents();
        SetupEventHandlers();
    }

    private void InitializeCustomComponents()
    {
        // Set form properties
        this.Text = "Hospital Management System";
        this.Size = new Size(1000, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        
        // Create title label
        lblTitle = new Label();
        lblTitle.Text = "Hospital Management System";
        lblTitle.Font = new Font("Arial", 20, FontStyle.Bold);
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblTitle.Dock = DockStyle.Top;
        lblTitle.Height = 50;
        lblTitle.BackColor = SystemColors.ControlLight;
        
        // Create navigation buttons
        btnPatients = new Button();
        btnPatients.Text = "Patients";
        btnPatients.Size = new Size(150, 40);
        btnPatients.Location = new Point(20, 80);
        btnPatients.BackColor = SystemColors.Control;
        btnPatients.Font = new Font("Arial", 10, FontStyle.Bold);
        
        btnDoctors = new Button();
        btnDoctors.Text = "Doctors";
        btnDoctors.Size = new Size(150, 40);
        btnDoctors.Location = new Point(190, 80);
        btnDoctors.BackColor = SystemColors.Control;
        btnDoctors.Font = new Font("Arial", 10, FontStyle.Bold);
        
        btnAppointments = new Button();
        btnAppointments.Text = "Appointments";
        btnAppointments.Size = new Size(150, 40);
        btnAppointments.Location = new Point(360, 80);
        btnAppointments.BackColor = SystemColors.Control;
        btnAppointments.Font = new Font("Arial", 10, FontStyle.Bold);
        
        // Create content panel
        pnlContent = new Panel();
        pnlContent.Location = new Point(20, 140);
        pnlContent.Size = new Size(940, 400);
        pnlContent.BorderStyle = BorderStyle.FixedSingle;
        
        // Add controls to form
        this.Controls.Add(lblTitle);
        this.Controls.Add(btnPatients);
        this.Controls.Add(btnDoctors);
        this.Controls.Add(btnAppointments);
        this.Controls.Add(pnlContent);
        
        // Show welcome message
        ShowWelcomeMessage();
    }
    
    private void SetupEventHandlers()
    {
        btnPatients.Click += (s, e) => ShowPatientManagement();
        btnDoctors.Click += (s, e) => ShowDoctorManagement();
        btnAppointments.Click += (s, e) => ShowAppointmentManagement();
    }
    
    private void ShowWelcomeMessage()
    {
        pnlContent.Controls.Clear();
        
        Label lblWelcome = new Label();
        lblWelcome.Text = "Welcome to the Hospital Management System";
        lblWelcome.Font = new Font("Arial", 16, FontStyle.Bold);
        lblWelcome.TextAlign = ContentAlignment.MiddleCenter;
        lblWelcome.Dock = DockStyle.Top;
        lblWelcome.Height = 50;
        
        Label lblInstructions = new Label();
        lblInstructions.Text = "This is a simplified hospital management system.\n\n" +
                             "Use the buttons above to navigate between different modules.\n\n" +
                             "• Patients: Manage patient records\n" +
                             "• Doctors: Manage doctor information\n" +
                             "• Appointments: Schedule and manage appointments";
        lblInstructions.Font = new Font("Arial", 12);
        lblInstructions.Location = new Point(20, 70);
        lblInstructions.Size = new Size(900, 300);
        
        pnlContent.Controls.Add(lblWelcome);
        pnlContent.Controls.Add(lblInstructions);
    }
    
    private void ShowPatientManagement()
    {
        pnlContent.Controls.Clear();
        PatientManagementControl patientControl = new PatientManagementControl();
        patientControl.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(patientControl);
    }
    
    private void ShowDoctorManagement()
    {
        pnlContent.Controls.Clear();
        DoctorManagementControl doctorControl = new DoctorManagementControl();
        doctorControl.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(doctorControl);
    }
    
    private void ShowAppointmentManagement()
    {
        pnlContent.Controls.Clear();
        AppointmentManagementControl appointmentControl = new AppointmentManagementControl();
        appointmentControl.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(appointmentControl);
    }
}
