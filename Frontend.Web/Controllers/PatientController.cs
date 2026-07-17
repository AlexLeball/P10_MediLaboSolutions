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
        public async Task<IActionResult> Create()
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null) return RedirectToAction("Login", "Auth");
            if (!JwtSessionHelper.CanManagePatients(token)) return Forbid();

            ViewBag.Practitioners = await _api.GetPractitionersAsync(token);
            return View(new PatientDto { BirthDate = DateTime.Today.AddDays(-1) });
        }

        [HttpPost]
        public async Task<IActionResult> Create(PatientDto dto)
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null) return RedirectToAction("Login", "Auth");
            if (!JwtSessionHelper.CanManagePatients(token)) return Forbid();

            if (!ModelState.IsValid)
            {
                // Repopulate dropdown on validation failure
                ViewBag.Practitioners = await _api.GetPractitionersAsync(token);
                return View(dto);
            }

            await _api.CreatePatientAsync(dto, token);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null) return RedirectToAction("Login", "Auth");
            if (!JwtSessionHelper.CanManagePatients(token)) return Forbid();

            var patient = await _api.GetPatientByIdAsync(id, token);
            if (patient == null) return NotFound();

            ViewBag.Practitioners = await _api.GetPractitionersAsync(token);
            return View(patient);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PatientDto dto)
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                // Repopulate dropdown on validation failure
                ViewBag.Practitioners = await _api.GetPractitionersAsync(token);
                return View(dto);
            }

            await _api.UpdatePatientAsync(dto, token);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddNote(MedicalNoteDto dto)
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null) return RedirectToAction("Login", "Auth");
            if (!JwtSessionHelper.CanViewNotes(token)) return Forbid();

            await _api.AddNoteAsync(dto, token);
            return RedirectToAction("Details", new { id = dto.PatientId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null) return RedirectToAction("Login", "Auth");

            var patient = await _api.GetPatientByIdAsync(id, token);
            var notes = JwtSessionHelper.CanViewNotes(token)
                ? await _api.GetNotesAsync(id, token)
                : new List<MedicalNoteDto>();

            var practitioners = await _api.GetPractitionersAsync(token);
            var practitionerName = practitioners
                .FirstOrDefault(p => p.Id == patient?.PractitionerId)
                ?.Name ?? "Unassigned";

            return View(new PatientDetailsViewModel
            {
                Patient = patient,
                Notes = notes,
                PractitionerName = practitionerName
            });
        }

        [HttpGet]
        public async Task<IActionResult> RiskReport(int id)
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null) return RedirectToAction("Login", "Auth");

            var report = await _api.GetRiskReportAsync(id, token);

            if (report == null) return NotFound();

            return View(report);
        }
    }
}