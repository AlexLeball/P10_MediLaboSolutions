namespace Frontend.Web.Models
{
    public class RiskReportDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public int TriggerCount { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
    }
}
