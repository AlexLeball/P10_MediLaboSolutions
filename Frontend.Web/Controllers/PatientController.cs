using Microsoft.AspNetCore.Mvc;
using Frontend.Web.Services;
using Frontend.Web.Models;

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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PatientDto dto)
        {
            var token = HttpContext.Session.GetString("JWT");

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
    }
}