namespace Notes.API.Entities
{
    public class MedicalNote
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public int PatientId { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Author { get; set; } = string.Empty; // doctor
    }
}
