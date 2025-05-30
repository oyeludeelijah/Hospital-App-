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
    public class AppointmentsController : ControllerBase
    {
        // Sample data for appointments (temporary until database is set up)
        private static readonly List<SimpleAppointment> _appointments = new List<SimpleAppointment>
        {
            new SimpleAppointment
            {
                Id = 1,
                PatientName = "Emily Davis",
                DoctorName = "Dr. Sarah Johnson",
                AppointmentDate = new DateTime(2025, 5, 30, 23, 28, 0),
                Purpose = "Follow-up",
                Status = "Scheduled"
            },
            new SimpleAppointment
            {
                Id = 2,
                PatientName = "Michael Brown",
                DoctorName = "Dr. John Smith",
                AppointmentDate = new DateTime(2025, 6, 1, 23, 28, 0),
                Purpose = "Regular checkup",
                Status = "Scheduled"
            }
        };

        // GET: api/Appointments
        [HttpGet]
        public ActionResult<IEnumerable<SimpleAppointment>> GetAll()
        {
            return Ok(_appointments);
        }

        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public ActionResult<SimpleAppointment> GetById(int id)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }
            return Ok(appointment);
        }

        // POST: api/Appointments
        [HttpPost]
        public ActionResult<SimpleAppointment> Create(SimpleAppointment appointment)
        {
            appointment.Id = _appointments.Count > 0 ? _appointments.Max(a => a.Id) + 1 : 1;
            _appointments.Add(appointment);
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
        }

        // PUT: api/Appointments/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, SimpleAppointment appointment)
        {
            if (id != appointment.Id)
            {
                return BadRequest();
            }

            var existingAppointment = _appointments.FirstOrDefault(a => a.Id == id);
            if (existingAppointment == null)
            {
                return NotFound();
            }

            // Update properties
            existingAppointment.PatientName = appointment.PatientName;
            existingAppointment.DoctorName = appointment.DoctorName;
            existingAppointment.AppointmentDate = appointment.AppointmentDate;
            existingAppointment.Purpose = appointment.Purpose;
            existingAppointment.Status = appointment.Status;

            return NoContent();
        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            _appointments.Remove(appointment);
            return NoContent();
        }
    }
}
