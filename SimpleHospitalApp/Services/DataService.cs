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

        // Singleton instance
        private static DataService? _instance;
        public static DataService Instance => _instance ??= new DataService();

        private DataService()
        {
            // Initialize with sample data
            _patients = new List<Patient>();
            _doctors = new List<Doctor>();
            _appointments = new List<Appointment>();

            SeedSampleData();
        }

        private void SeedSampleData()
        {
            // Sample doctors
            _doctors.Add(new Doctor
            {
                Id = 1,
                FirstName = "John",
                LastName = "Smith",
                Specialization = "Cardiology",
                ContactNumber = "555-1234",
                Email = "john.smith@hospital.com"
            });

            _doctors.Add(new Doctor
            {
                Id = 2,
                FirstName = "Sarah",
                LastName = "Johnson",
                Specialization = "Pediatrics",
                ContactNumber = "555-5678",
                Email = "sarah.johnson@hospital.com"
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
    }
}