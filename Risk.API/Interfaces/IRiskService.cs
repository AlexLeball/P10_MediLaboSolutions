using Risk.API.DTOs;

namespace Risk.API.Interfaces
{
    public interface IRiskService
    {
        Task<RiskReportDto?> GetRiskReportAsync(int patientId, string bearerToken);
    }
}