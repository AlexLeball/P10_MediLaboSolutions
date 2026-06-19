using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Notes.API.Models
{
    public class MedicalNote
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("patientId")]
        public int PatientId { get; set; }

        [BsonElement("content")]
        public string Content { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("author")]
        public string Author { get; set; } = string.Empty;
    }
}
