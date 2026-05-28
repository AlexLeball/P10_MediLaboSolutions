using Microsoft.AspNetCore.Authorization;
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

        public PatientController(IPatientService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            Console.WriteLine("CONTROLLER HIT");
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var patient = _service.GetById(id);
            if (patient == null)
                return NotFound();
            return Ok(patient);
        }

        [HttpPost]
        public IActionResult Create(PatientEntity patient)
        {
            _service.Add(patient);
            return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
        }

        [HttpPut]
        public IActionResult Update(PatientEntity patient)
        {
            _service.Update(patient);
            return Ok();
        }
    }
}