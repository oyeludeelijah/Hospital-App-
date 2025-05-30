using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HospitalApp.Web.HospitalApi.Models;
using HospitalApp.Web.HospitalApi.Data;

namespace HospitalApp.Web.HospitalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sample data for doctors (temporary until database is set up)
        private static readonly List<SimpleDoctor> _doctors = new List<SimpleDoctor>
        {
            new SimpleDoctor
            {
                Id = 1,
                FirstName = "John",
                LastName = "Smith",
                Specialization = "Dermatology",
                ContactNumber = "555-1234",
                Email = "john.smith@hospital.com",
                Department = "Cardiology"
            },
            new SimpleDoctor
            {
                Id = 2,
                FirstName = "Sarah",
                LastName = "Johnson",
                Specialization = "Pediatrics",
                ContactNumber = "555-5678",
                Email = "sarah.johnson@hospital.com",
                Department = "Pediatrics"
            }
        };

        // GET: api/Doctors
        [HttpGet]
        public ActionResult<IEnumerable<SimpleDoctor>> GetAll()
        {
            return Ok(_doctors);
        }

        // GET: api/Doctors/5
        [HttpGet("{id}")]
        public ActionResult<SimpleDoctor> GetById(int id)
        {
            var doctor = _doctors.FirstOrDefault(d => d.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor);
        }

        // POST: api/Doctors
        [HttpPost]
        public ActionResult<SimpleDoctor> Create(SimpleDoctor doctor)
        {
            doctor.Id = _doctors.Count > 0 ? _doctors.Max(d => d.Id) + 1 : 1;
            _doctors.Add(doctor);
            return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, doctor);
        }

        // PUT: api/Doctors/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, SimpleDoctor doctor)
        {
            if (id != doctor.Id)
            {
                return BadRequest();
            }

            var existingDoctor = _doctors.FirstOrDefault(d => d.Id == id);
            if (existingDoctor == null)
            {
                return NotFound();
            }

            // Update properties
            existingDoctor.FirstName = doctor.FirstName;
            existingDoctor.LastName = doctor.LastName;
            existingDoctor.Specialization = doctor.Specialization;
            existingDoctor.ContactNumber = doctor.ContactNumber;
            existingDoctor.Email = doctor.Email;
            existingDoctor.Department = doctor.Department;

            return NoContent();
        }

        // DELETE: api/Doctors/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var doctor = _doctors.FirstOrDefault(d => d.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            _doctors.Remove(doctor);
            return NoContent();
        }
    }
}
