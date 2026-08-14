using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Patient.Application.Interfaces;
using Patient.Domain.Entities;
using System.Security.Claims;

namespace Patient.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientController(IPatientService service) => _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin,Organiser,Practitioner")]
        public IActionResult GetAll()
        {
            // Practitioner only sees their own patients
            if (User.IsInRole("Practitioner"))
            {
                var practitionerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Ok(_service.GetByPractitionerId(practitionerId!));
            }

            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Organiser,Practitioner")]
        public IActionResult GetById(int id)
        {
            var patient = _service.GetById(id);

            if (patient == null)
                return NotFound();

            // Practitioner can only access their own patients
            if (User.IsInRole("Practitioner"))
            {
                var practitionerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (patient.PractitionerId != practitionerId)
                    return Forbid();
            }

            return Ok(patient);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Organiser")]
        public IActionResult Create(PatientEntity patient)
        {
            _service.Add(patient);
            return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Organiser")]
        public IActionResult Update(int id, [FromBody] PatientEntity patient)
        {
            if (id != patient.Id) return BadRequest();
            _service.Update(patient);
            return Ok();
        }
    }
}