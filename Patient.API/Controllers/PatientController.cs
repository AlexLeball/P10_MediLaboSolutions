using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Patient.Application.Interfaces;
using Patient.Domain.Entities;

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
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Organiser,Practitioner")]
        public IActionResult GetById(int id)
        {
            var patient = _service.GetById(id);
            return patient == null ? NotFound() : Ok(patient);
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