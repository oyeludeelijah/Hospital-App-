using HospitalApp.Core.Models;
using HospitalApp.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace HospitalApp.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Patient> Patients { get; }
        IRepository<Doctor> Doctors { get; }
        IRepository<Nurse> Nurses { get; }
        IRepository<Appointment> Appointments { get; }
        IRepository<MedicalRecord> MedicalRecords { get; }
        IRepository<Prescription> Prescriptions { get; }
        IRepository<PrescriptionItem> PrescriptionItems { get; }
        IRepository<Medication> Medications { get; }
        IRepository<LabTest> LabTests { get; }
        IRepository<Admission> Admissions { get; }
        IRepository<Room> Rooms { get; }
        IRepository<Bill> Bills { get; }
        IRepository<BillItem> BillItems { get; }
        IRepository<Payment> Payments { get; }
        IRepository<DoctorSchedule> DoctorSchedules { get; }
        
        Task<int> CompleteAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        
        public IRepository<User> Users { get; }
        public IRepository<Patient> Patients { get; }
        public IRepository<Doctor> Doctors { get; }
        public IRepository<Nurse> Nurses { get; }
        public IRepository<Appointment> Appointments { get; }
        public IRepository<MedicalRecord> MedicalRecords { get; }
        public IRepository<Prescription> Prescriptions { get; }
        public IRepository<PrescriptionItem> PrescriptionItems { get; }
        public IRepository<Medication> Medications { get; }
        public IRepository<LabTest> LabTests { get; }
        public IRepository<Admission> Admissions { get; }
        public IRepository<Room> Rooms { get; }
        public IRepository<Bill> Bills { get; }
        public IRepository<BillItem> BillItems { get; }
        public IRepository<Payment> Payments { get; }
        public IRepository<DoctorSchedule> DoctorSchedules { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            
            Users = new Repository<User>(context);
            Patients = new Repository<Patient>(context);
            Doctors = new Repository<Doctor>(context);
            Nurses = new Repository<Nurse>(context);
            Appointments = new Repository<Appointment>(context);
            MedicalRecords = new Repository<MedicalRecord>(context);
            Prescriptions = new Repository<Prescription>(context);
            PrescriptionItems = new Repository<PrescriptionItem>(context);
            Medications = new Repository<Medication>(context);
            LabTests = new Repository<LabTest>(context);
            Admissions = new Repository<Admission>(context);
            Rooms = new Repository<Room>(context);
            Bills = new Repository<Bill>(context);
            BillItems = new Repository<BillItem>(context);
            Payments = new Repository<Payment>(context);
            DoctorSchedules = new Repository<DoctorSchedule>(context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
