using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using HospitalApp.Web.HospitalApi.Models;
using HospitalApp.Web.HospitalApi.Data;

namespace HospitalApp.Web.HospitalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        // Sample data for departments (temporary until database is set up)
        private static readonly List<SimpleDepartment> _departments = new List<SimpleDepartment>
        {
            new SimpleDepartment
            {
                Id = 1,
                Name = "Cardiology",
                Description = "Heart and cardiovascular system",
                DoctorCount = 1
            },
            new SimpleDepartment
            {
                Id = 2,
                Name = "Pediatrics",
                Description = "Medical care for infants, children",
                DoctorCount = 1
            },
            new SimpleDepartment
            {
                Id = 3,
                Name = "Orthopedics",
                Description = "Musculoskeletal system - bones",
                DoctorCount = 0
            }
        };

        // GET: api/Departments
        [HttpGet]
        public ActionResult<IEnumerable<SimpleDepartment>> GetAll()
        {
            return Ok(_departments);
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public ActionResult<SimpleDepartment> GetById(int id)
        {
            var department = _departments.FirstOrDefault(d => d.Id == id);
            if (department == null)
            {
                return NotFound();
            }
            return Ok(department);
        }

        // POST: api/Departments
        [HttpPost]
        public ActionResult<SimpleDepartment> Create(SimpleDepartment department)
        {
            department.Id = _departments.Count > 0 ? _departments.Max(d => d.Id) + 1 : 1;
            _departments.Add(department);
            return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
        }

        // PUT: api/Departments/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, SimpleDepartment department)
        {
            if (id != department.Id)
            {
                return BadRequest();
            }

            var existingDepartment = _departments.FirstOrDefault(d => d.Id == id);
            if (existingDepartment == null)
            {
                return NotFound();
            }

            // Update properties
            existingDepartment.Name = department.Name;
            existingDepartment.Description = department.Description;
            existingDepartment.DoctorCount = department.DoctorCount;

            return NoContent();
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var department = _departments.FirstOrDefault(d => d.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            _departments.Remove(department);
            return NoContent();
        }
    }
}
