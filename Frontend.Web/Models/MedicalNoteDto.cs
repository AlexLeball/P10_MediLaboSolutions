namespace Frontend.Web.Models
{
    public class MedicalNoteDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public int PatientId { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Author { get; set; } = string.Empty;
    }
}
