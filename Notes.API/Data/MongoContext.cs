using MongoDB.Driver;
using Notes.API.Models;

namespace Notes.API.Data
{
    public class MongoContext
    {
        private readonly IMongoDatabase _db;

        public MongoContext(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDb:ConnectionString"]);
            _db = client.GetDatabase(config["MongoDb:DatabaseName"]);
        }

        public IMongoCollection<MedicalNote> Notes =>
            _db.GetCollection<MedicalNote>("MedicalNotes");
    }
}

