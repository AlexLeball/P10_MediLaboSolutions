using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.API.Models;
using Notes.API.Services;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly NotesService _service;

    public NotesController(NotesService service)
    {
        _service = service;
    }

    [HttpGet("{patientId}")]
    public async Task<IActionResult> GetByPatient(int patientId)
    {
        return Ok(await _service.GetByPatientId(patientId));
    }

    [HttpPost]
    public async Task<IActionResult> Create(MedicalNote note)
    {
        await _service.Create(note);
        return Created("", note);
    }
}
