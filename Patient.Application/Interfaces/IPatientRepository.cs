using Patient.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Patient.Application.Interfaces
{
    public interface IPatientRepository
    {
        List<PatientEntity> GetAll();
        List<PatientEntity> GetByPractitionerId(string practitionerId);
        PatientEntity? GetById(int id);
        void Add(PatientEntity patient);
        void Update(PatientEntity patient);
    }
}