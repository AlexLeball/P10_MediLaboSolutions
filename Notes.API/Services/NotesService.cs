using Notes.API.Data;
using Notes.API.Models;
using MongoDB.Driver;

namespace Notes.API.Services
{
    public class NotesService
    {
        private readonly MongoContext _context;

        public NotesService(MongoContext context)
        {
            _context = context;
        }

        public async Task<List<MedicalNote>> GetByPatientId(int patientId)
        {
            return await _context.Notes
                .Find(n => n.PatientId == patientId)
                .SortByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task Create(MedicalNote note)
        {
            await _context.Notes.InsertOneAsync(note);
        }
        
    }
}
