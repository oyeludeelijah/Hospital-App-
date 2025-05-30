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
    public class BillingsController : ControllerBase
    {
        // Sample data for billings (temporary until database is set up)
        private static readonly List<SimpleBilling> _billings = new List<SimpleBilling>
        {
            new SimpleBilling
            {
                Id = 1,
                PatientName = "Michael Brown",
                Amount = 350.00m,
                BillingDate = new DateTime(2025, 5, 15),
                PaymentStatus = "Paid",
                PaymentMethod = "Credit Card"
            },
            new SimpleBilling
            {
                Id = 2,
                PatientName = "Emily Davis",
                Amount = 500.00m,
                BillingDate = new DateTime(2025, 5, 20),
                PaymentStatus = "Pending",
                PaymentMethod = "Insurance"
            }
        };

        // GET: api/Billings
        [HttpGet]
        public ActionResult<IEnumerable<SimpleBilling>> GetAll()
        {
            return Ok(_billings);
        }

        // GET: api/Billings/5
        [HttpGet("{id}")]
        public ActionResult<SimpleBilling> GetById(int id)
        {
            var billing = _billings.FirstOrDefault(b => b.Id == id);
            if (billing == null)
            {
                return NotFound();
            }
            return Ok(billing);
        }

        // POST: api/Billings
        [HttpPost]
        public ActionResult<SimpleBilling> Create(SimpleBilling billing)
        {
            billing.Id = _billings.Count > 0 ? _billings.Max(b => b.Id) + 1 : 1;
            _billings.Add(billing);
            return CreatedAtAction(nameof(GetById), new { id = billing.Id }, billing);
        }

        // PUT: api/Billings/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, SimpleBilling billing)
        {
            if (id != billing.Id)
            {
                return BadRequest();
            }

            var existingBilling = _billings.FirstOrDefault(b => b.Id == id);
            if (existingBilling == null)
            {
                return NotFound();
            }

            // Update properties
            existingBilling.PatientName = billing.PatientName;
            existingBilling.Amount = billing.Amount;
            existingBilling.BillingDate = billing.BillingDate;
            existingBilling.PaymentStatus = billing.PaymentStatus;
            existingBilling.PaymentMethod = billing.PaymentMethod;

            return NoContent();
        }

        // DELETE: api/Billings/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var billing = _billings.FirstOrDefault(b => b.Id == id);
            if (billing == null)
            {
                return NotFound();
            }

            _billings.Remove(billing);
            return NoContent();
        }
    }
}
