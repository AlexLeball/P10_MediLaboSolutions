namespace Risk.API.DTOs
{
    public class MedicalNoteDto
    {
        public string Id { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}