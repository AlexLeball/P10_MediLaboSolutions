using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.API.Models;
using Notes.API.Services;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly NotesService _service;

    public NotesController(NotesService service) => _service = service;

    [HttpGet("{patientId}")]
    [Authorize(Roles = "Admin,Organiser,Practitioner")]
    public async Task<IActionResult> GetByPatient(int patientId)
        => Ok(await _service.GetByPatientId(patientId));

    [HttpPost]
    [Authorize(Roles = "Admin,Practitioner")]
    public async Task<IActionResult> Create(MedicalNote note)
    {
        note.Author = User.FindFirstValue(ClaimTypes.Name)
                      ?? User.FindFirstValue(ClaimTypes.Email)
                      ?? "Unknown";
        await _service.Create(note);
        return Created("", note);
    }
}
