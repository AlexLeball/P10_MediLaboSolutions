using Patient.Domain.Entities;

namespace Patient.Application.Interfaces
{
    public interface IPatientService
    {
        List<PatientEntity> GetAll();

        PatientEntity GetById(int id);

        void Add(PatientEntity patient);

        void Update(PatientEntity patient);
    }
}