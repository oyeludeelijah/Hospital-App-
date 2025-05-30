using HospitalApp.Web.HospitalApi.Models;
using HospitalApp.Web.HospitalApi.Repositories;
using System;
using System.Threading.Tasks;

namespace HospitalApp.Web.HospitalApi.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Patient> Patients { get; }
        
        Task<int> CompleteAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        
        public IRepository<Patient> Patients { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            
            Patients = new Repository<Patient>(context);
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
