using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
