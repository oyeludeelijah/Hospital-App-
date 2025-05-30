using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using HospitalApp.Web.HospitalApi.Models;

namespace HospitalApp.Web.HospitalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        // Sample data for testing
        private static readonly List<Patient> _patients = new List<Patient>
        {
            new Patient 
            { 
                Id = 1, 
                FirstName = "John", 
                LastName = "Doe", 
                DateOfBirth = new System.DateTime(1980, 1, 1),
                Gender = "Male",
                ContactNumber = "555-1234",
                Email = "john.doe@example.com",
                Address = "123 Main St"
            },
            new Patient 
            { 
                Id = 2, 
                FirstName = "Jane", 
                LastName = "Smith", 
                DateOfBirth = new System.DateTime(1985, 5, 15),
                Gender = "Female",
                ContactNumber = "555-5678",
                Email = "jane.smith@example.com",
                Address = "456 Oak Ave"
            }
        };

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_patients);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var patient = _patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }
    }
}
