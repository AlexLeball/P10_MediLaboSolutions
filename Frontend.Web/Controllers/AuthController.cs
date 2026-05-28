using Microsoft.AspNetCore.Mvc;
using Frontend.Web.Services;
using Frontend.Web.Models;

namespace Frontend.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly PatientApiService _api;

        public AuthController(PatientApiService api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = await _api.LoginAsync(model.Email, model.Password);

            if (token == null)
            {
                ModelState.AddModelError("", "Invalid login");
                return View(model);
            }

            HttpContext.Session.SetString("JWT", token);

            return RedirectToAction("Index", "Patient");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWT");
            return RedirectToAction("Login");
        }
    }
}