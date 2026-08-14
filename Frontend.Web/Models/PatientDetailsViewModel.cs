namespace Frontend.Web.Models
{
    public class PatientDetailsViewModel
    {
        public PatientDto Patient { get; set; } = new();
        public List<MedicalNoteDto> Notes { get; set; } = new();
        public string PractitionerName { get; set; } = "Unassigned";
        public RiskReportDto? RiskReport { get; set; }
    }
}
