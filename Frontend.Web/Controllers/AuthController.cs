using Frontend.Web.Helpers;
using Frontend.Web.Models;
using Frontend.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly PatientApiService _api;

        public AuthController(PatientApiService api) => _api = api;

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

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
            if (token == null) return RedirectToAction("Login");
            if (!JwtSessionHelper.IsAdmin(token)) return Forbid();

            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (token == null) return RedirectToAction("Login");
            if (!JwtSessionHelper.IsAdmin(token)) return Forbid();

            if (!ModelState.IsValid) return View(model);

            var (success, error) = await _api.RegisterAsync(
                model.Email, model.Password, model.Role, model.FullName, token);

            if (!success)
            {
                ModelState.AddModelError("", error ?? "Registration failed.");
                return View(model);
            }

            TempData["Success"] = $"User '{model.FullName}' created with role '{model.Role}'.";
            return RedirectToAction("Register");
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (token == null) return RedirectToAction("Login");
            if (!JwtSessionHelper.IsAdmin(token)) return Forbid();

            var users = await _api.GetUsersAsync(token);
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (token == null) return RedirectToAction("Login");
            if (!JwtSessionHelper.IsAdmin(token)) return Forbid();

            var user = await _api.GetUserByIdAsync(id, token);
            if (user == null) return NotFound();

            return View(new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (token == null) return RedirectToAction("Login");
            if (!JwtSessionHelper.IsAdmin(token)) return Forbid();

            if (!ModelState.IsValid) return View(model);

            var (success, error) = await _api.UpdateUserAsync(model, token);

            if (!success)
            {
                ModelState.AddModelError("", error ?? "Update failed.");
                return View(model);
            }

            TempData["Success"] = $"User '{model.FullName}' updated successfully.";
            return RedirectToAction("Users");
        }
    }
}