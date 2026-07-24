using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Risk.API.Interfaces;

namespace Risk.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RiskController : ControllerBase
    {
        private readonly IRiskService _riskService;

        public RiskController(IRiskService riskService) => _riskService = riskService;

        /// <summary>GET /api/risk/{patientId}</summary>
        [HttpGet("{patientId:int}")]
        public async Task<IActionResult> GetRiskReport(int patientId)
        {
            // Extraire le token reçu et le transmettre aux APIs amont
            var authHeader = Request.Headers.Authorization.ToString();
            var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authHeader["Bearer ".Length..]
                : string.Empty;

            var report = await _riskService.GetRiskReportAsync(patientId, token);

            if (report is null)
                return NotFound();

            return Ok(report);
        }
    }
}
