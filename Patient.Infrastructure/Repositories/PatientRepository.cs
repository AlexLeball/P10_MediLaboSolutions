using Patient.Application.Interfaces;
using Patient.Domain.Entities;
using Patient.Infrastructure.Data;

namespace Patient.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<PatientEntity> GetAll() => _context.Patients.ToList();

        public List<PatientEntity> GetByPractitionerId(string practitionerId) =>
            _context.Patients
                .Where(p => p.PractitionerId == practitionerId)
                .ToList();

        public PatientEntity? GetById(int id) => _context.Patients.Find(id);

        public void Add(PatientEntity patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }

        public void Update(PatientEntity patient)
        {
            _context.Patients.Update(patient);
            _context.SaveChanges();
        }
    }
}
