using SimpleHospitalApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleHospitalApp.Services
{
    public class DataService
    {
        // In-memory collections
        private List<Patient> _patients;
        private List<Doctor> _doctors;
        private List<Appointment> _appointments;
        private List<Billing> _billings;
        private List<Department> _departments;
        private List<MedicalRecord> _medicalRecords;

        // Singleton instance
        private static DataService? _instance;
        public static DataService Instance => _instance ??= new DataService();

        private DataService()
        {
            // Initialize with sample data
            _patients = new List<Patient>();
            _doctors = new List<Doctor>();
            _appointments = new List<Appointment>();
            _billings = new List<Billing>();
            _departments = new List<Department>();
            _medicalRecords = new List<MedicalRecord>();

            SeedSampleData();
        }

        private void SeedSampleData()
        {
            // Sample departments
            _departments.Add(new Department
            {
                Id = 1,
                Name = "Cardiology",
                Description = "Heart and cardiovascular system"
            });
            
            _departments.Add(new Department
            {
                Id = 2,
                Name = "Pediatrics",
                Description = "Medical care for infants, children, and adolescents"
            });
            
            _departments.Add(new Department
            {
                Id = 3,
                Name = "Orthopedics",
                Description = "Musculoskeletal system - bones, joints, ligaments"
            });
            
            // Sample doctors
            _doctors.Add(new Doctor
            {
                Id = 1,
                FirstName = "John",
                LastName = "Smith",
                Specialization = "Cardiology",
                ContactNumber = "555-1234",
                Email = "john.smith@hospital.com",
                DepartmentId = 1
            });

            _doctors.Add(new Doctor
            {
                Id = 2,
                FirstName = "Sarah",
                LastName = "Johnson",
                Specialization = "Pediatrics",
                ContactNumber = "555-5678",
                Email = "sarah.johnson@hospital.com",
                DepartmentId = 2
            });

            // Sample patients
            _patients.Add(new Patient
            {
                Id = 1,
                FirstName = "Michael",
                LastName = "Brown",
                DateOfBirth = new DateTime(1985, 4, 15),
                Gender = "Male",
                ContactNumber = "555-9876",
                Email = "michael.brown@email.com",
                Address = "123 Main St",
                MedicalHistory = "Hypertension"
            });

            _patients.Add(new Patient
            {
                Id = 2,
                FirstName = "Emily",
                LastName = "Davis",
                DateOfBirth = new DateTime(1990, 8, 22),
                Gender = "Female",
                ContactNumber = "555-5432",
                Email = "emily.davis@email.com",
                Address = "456 Oak Ave",
                MedicalHistory = "Asthma"
            });

            // Sample appointments
            _appointments.Add(new Appointment
            {
                Id = 1,
                PatientId = 1,
                DoctorId = 1,
                AppointmentDate = DateTime.Now.AddDays(3),
                Purpose = "Regular checkup",
                Status = "Scheduled"
            });

            _appointments.Add(new Appointment
            {
                Id = 2,
                PatientId = 2,
                DoctorId = 2,
                AppointmentDate = DateTime.Now.AddDays(1),
                Purpose = "Follow-up",
                Status = "Scheduled"
            });
        }

        // Patient methods
        public List<Patient> GetAllPatients() => _patients;

        public Patient? GetPatientById(int id) => _patients.FirstOrDefault(p => p.Id == id);

        public void AddPatient(Patient patient)
        {
            patient.Id = _patients.Count > 0 ? _patients.Max(p => p.Id) + 1 : 1;
            _patients.Add(patient);
        }

        public void UpdatePatient(Patient patient)
        {
            var existingPatient = _patients.FirstOrDefault(p => p.Id == patient.Id);
            if (existingPatient != null)
            {
                int index = _patients.IndexOf(existingPatient);
                _patients[index] = patient;
            }
        }

        public void DeletePatient(int id)
        {
            var patient = _patients.FirstOrDefault(p => p.Id == id);
            if (patient != null)
            {
                _patients.Remove(patient);
            }
        }

        // Doctor methods
        public List<Doctor> GetAllDoctors() => _doctors;

        public Doctor? GetDoctorById(int id) => _doctors.FirstOrDefault(d => d.Id == id);

        public void AddDoctor(Doctor doctor)
        {
            doctor.Id = _doctors.Count > 0 ? _doctors.Max(d => d.Id) + 1 : 1;
            _doctors.Add(doctor);
        }

        public void UpdateDoctor(Doctor doctor)
        {
            var existingDoctor = _doctors.FirstOrDefault(d => d.Id == doctor.Id);
            if (existingDoctor != null)
            {
                int index = _doctors.IndexOf(existingDoctor);
                _doctors[index] = doctor;
            }
        }

        public void DeleteDoctor(int id)
        {
            var doctor = _doctors.FirstOrDefault(d => d.Id == id);
            if (doctor != null)
            {
                _doctors.Remove(doctor);
            }
        }

        // Appointment methods
        public List<Appointment> GetAllAppointments() => _appointments;

        public Appointment? GetAppointmentById(int id) => _appointments.FirstOrDefault(a => a.Id == id);

        public List<Appointment> GetAppointmentsByPatient(int patientId) => 
            _appointments.Where(a => a.PatientId == patientId).ToList();

        public List<Appointment> GetAppointmentsByDoctor(int doctorId) => 
            _appointments.Where(a => a.DoctorId == doctorId).ToList();

        public void AddAppointment(Appointment appointment)
        {
            appointment.Id = _appointments.Count > 0 ? _appointments.Max(a => a.Id) + 1 : 1;
            _appointments.Add(appointment);
        }

        public void UpdateAppointment(Appointment appointment)
        {
            var existingAppointment = _appointments.FirstOrDefault(a => a.Id == appointment.Id);
            if (existingAppointment != null)
            {
                int index = _appointments.IndexOf(existingAppointment);
                _appointments[index] = appointment;
            }
        }

        public void DeleteAppointment(int id)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                _appointments.Remove(appointment);
            }
        }
        
        // Billing methods
        public List<Billing> GetAllBillings() => _billings;
        
        public Billing? GetBillingById(int id) => _billings.FirstOrDefault(b => b.Id == id);
        
        public List<Billing> GetBillingsByPatient(int patientId) => 
            _billings.Where(b => b.PatientId == patientId).ToList();
        
        public void AddBilling(Billing billing)
        {
            billing.Id = _billings.Count > 0 ? _billings.Max(b => b.Id) + 1 : 1;
            _billings.Add(billing);
        }
        
        public void UpdateBilling(Billing billing)
        {
            var existingBilling = _billings.FirstOrDefault(b => b.Id == billing.Id);
            if (existingBilling != null)
            {
                int index = _billings.IndexOf(existingBilling);
                _billings[index] = billing;
            }
        }
        
        public void DeleteBilling(int id)
        {
            var billing = _billings.FirstOrDefault(b => b.Id == id);
            if (billing != null)
            {
                _billings.Remove(billing);
            }
        }
        
        // Department methods
        public List<Department> GetAllDepartments() => _departments;
        
        public Department? GetDepartmentById(int id) => _departments.FirstOrDefault(d => d.Id == id);
        
        public List<Doctor> GetDoctorsByDepartment(int departmentId) => 
            _doctors.Where(d => d.DepartmentId == departmentId).ToList();
        
        public void AddDepartment(Department department)
        {
            department.Id = _departments.Count > 0 ? _departments.Max(d => d.Id) + 1 : 1;
            _departments.Add(department);
        }
        
        public void UpdateDepartment(Department department)
        {
            var existingDepartment = _departments.FirstOrDefault(d => d.Id == department.Id);
            if (existingDepartment != null)
            {
                int index = _departments.IndexOf(existingDepartment);
                _departments[index] = department;
            }
        }
        
        public void DeleteDepartment(int id)
        {
            var department = _departments.FirstOrDefault(d => d.Id == id);
            if (department != null)
            {
                _departments.Remove(department);
            }
        }
        
        // Medical Record methods
        public List<MedicalRecord> GetAllMedicalRecords() => _medicalRecords;
        
        public MedicalRecord? GetMedicalRecordById(int id) => _medicalRecords.FirstOrDefault(m => m.Id == id);
        
        public List<MedicalRecord> GetMedicalRecordsByPatient(int patientId) => 
            _medicalRecords.Where(m => m.PatientId == patientId).ToList();
        
        public void AddMedicalRecord(MedicalRecord record)
        {
            record.Id = _medicalRecords.Count > 0 ? _medicalRecords.Max(m => m.Id) + 1 : 1;
            _medicalRecords.Add(record);
        }
        
        public void UpdateMedicalRecord(MedicalRecord record)
        {
            var existingRecord = _medicalRecords.FirstOrDefault(m => m.Id == record.Id);
            if (existingRecord != null)
            {
                int index = _medicalRecords.IndexOf(existingRecord);
                _medicalRecords[index] = record;
            }
        }
        
        public void DeleteMedicalRecord(int id)
        {
            var record = _medicalRecords.FirstOrDefault(m => m.Id == id);
            if (record != null)
            {
                _medicalRecords.Remove(record);
            }
        }
    }
}