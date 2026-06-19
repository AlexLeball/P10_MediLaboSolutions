using Microsoft.AspNetCore.Mvc;
using Frontend.Web.Services;
using Frontend.Web.Models;
using Frontend.Web.Helpers;

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
        public IActionResult Login() => View();

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

        [HttpGet]
        public IActionResult Register()
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null)
                return RedirectToAction("Login");

            if (!JwtSessionHelper.IsAdmin(token))
                return Forbid();

            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var token = HttpContext.Session.GetString("JWT");

            if (token == null)
                return RedirectToAction("Login");

            if (!JwtSessionHelper.IsAdmin(token))
                return Forbid();

            if (!ModelState.IsValid)
                return View(model);

            var (success, error) = await _api.RegisterAsync(
                model.Email, model.Password, model.Role, token);

            if (!success)
            {
                ModelState.AddModelError("", error ?? "Registration failed.");
                return View(model);
            }

            TempData["Success"] = $"User '{model.Email}' created with role '{model.Role}'.";
            return RedirectToAction("Register");
        }
    }
}