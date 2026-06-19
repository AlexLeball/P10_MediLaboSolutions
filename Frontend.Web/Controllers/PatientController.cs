using Frontend.Web.Helpers;
using Frontend.Web.Models;
using Frontend.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Web.Controllers
{
    public class PatientController : Controller
    {
        private readonly PatientApiService _api;

        public PatientController(PatientApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null)
                return RedirectToAction("Login", "Auth");

            var patients = await _api.GetPatientsAsync(token);

            return View(patients);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null)
                return RedirectToAction("Login", "Auth");

            if (!JwtSessionHelper.CanManagePatients(token))
                return Forbid();

            return View(new PatientDto { BirthDate = DateTime.Today.AddDays(-1) });
        }

        [HttpPost]
        public async Task<IActionResult> Create(PatientDto dto)
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null)
                return RedirectToAction("Login", "Auth");

            if (!JwtSessionHelper.CanManagePatients(token))
                return Forbid();

            if (!ModelState.IsValid)
                return View(dto);

            await _api.CreatePatientAsync(dto, token);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JWT");

            var patient = await _api.GetPatientByIdAsync(id, token);
            return View(patient);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PatientDto dto)
        {
            var token = HttpContext.Session.GetString("JWT");

            await _api.UpdatePatientAsync(dto, token);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddNote(MedicalNoteDto dto)
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null)
                return RedirectToAction("Login", "Auth");

            // Organisers cannot add notes
            if (!JwtSessionHelper.CanViewNotes(token))
                return Forbid();

            await _api.AddNoteAsync(dto, token);
            return RedirectToAction("Details", new { id = dto.PatientId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null)
                return RedirectToAction("Login", "Auth");

            var patient = await _api.GetPatientByIdAsync(id, token);

            // Only fetch notes for roles that are allowed to see them
            var notes = JwtSessionHelper.CanViewNotes(token)
                ? await _api.GetNotesAsync(id, token)
                : new List<MedicalNoteDto>();

            return View(new PatientDetailsViewModel
            {
                Patient = patient,
                Notes = notes
            });
        }
    }
}