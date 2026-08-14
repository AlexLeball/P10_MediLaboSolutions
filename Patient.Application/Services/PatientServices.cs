using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Patient.Application.Interfaces;
using Patient.Domain.Entities;

namespace Patient.Application.Services
{

    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;

        public PatientService(IPatientRepository repository)
        {
            _repository = repository;
        }

        public List<PatientEntity> GetAll() => _repository.GetAll();

        public List<PatientEntity> GetByPractitionerId(string practitionerId) =>
            _repository.GetByPractitionerId(practitionerId);

        public PatientEntity? GetById(int id) => _repository.GetById(id);

        public void Add(PatientEntity patient) => _repository.Add(patient);

        public void Update(PatientEntity patient) => _repository.Update(patient);
    }

}
