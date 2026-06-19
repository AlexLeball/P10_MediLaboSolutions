using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Patient.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Patient.API.Controllers
{
    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string Email, string Password, string Role, string FullName);
    public record UpdateUserRequest(string FullName, string Email, string Role, string? NewPassword);

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var allowedRoles = new[] { "Organiser", "Practitioner" };
            if (!allowedRoles.Contains(request.Role))
                return BadRequest($"Role must be one of: {string.Join(", ", allowedRoles)}");

            var user = new ApplicationUser { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, request.Role);
            await _userManager.AddClaimAsync(user, new Claim("FullName", request.FullName));

            return Ok($"User created with role '{request.Role}'");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return Unauthorized();

            return Ok(new { token = await GenerateJwtToken(user) });
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var result = new List<object>();

            foreach (var role in new[] { "Organiser", "Practitioner" })
            {
                var users = await _userManager.GetUsersInRoleAsync(role);
                foreach (var u in users)
                {
                    var claims = await _userManager.GetClaimsAsync(u);
                    var fullName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value
                                   ?? u.UserName!;
                    result.Add(new { u.Id, FullName = fullName, Email = u.Email, Role = role });
                }
            }

            return Ok(result);
        }

        [HttpGet("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            var fullName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value
                           ?? user.UserName!;

            return Ok(new
            {
                user.Id,
                FullName = fullName,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? ""
            });
        }

        [HttpPut("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Update email / username
            user.Email = request.Email;
            user.UserName = request.Email;
            await _userManager.UpdateAsync(user);

            // Update FullName claim
            var claims = await _userManager.GetClaimsAsync(user);
            var existing = claims.FirstOrDefault(c => c.Type == "FullName");
            if (existing != null)
                await _userManager.ReplaceClaimAsync(user, existing,
                    new Claim("FullName", request.FullName));
            else
                await _userManager.AddClaimAsync(user,
                    new Claim("FullName", request.FullName));

            // Update role
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.Role);

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }

            return Ok("User updated");
        }

        [HttpGet("practitioners")]
        [Authorize(Roles = "Admin,Organiser")]
        public async Task<IActionResult> GetPractitioners()
        {
            var practitioners = await _userManager.GetUsersInRoleAsync("Practitioner");
            var result = new List<object>();

            foreach (var u in practitioners)
            {
                var claims = await _userManager.GetClaimsAsync(u);
                var fullName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value
                               ?? u.UserName!;
                result.Add(new { u.Id, Name = fullName });
            }

            return Ok(result);
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name,
                    userClaims.FirstOrDefault(c => c.Type == "FullName")?.Value
                    ?? user.UserName!)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(_config["Jwt:DurationInMinutes"]!)),
                signingCredentials: creds));
        }
    }
}