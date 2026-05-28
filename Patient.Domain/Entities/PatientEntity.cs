using System;
using System.ComponentModel.DataAnnotations;

namespace Patient.Domain.Entities
{
    public class PatientEntity
    {
        public int Id { get; set; }

        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string LastName { get; set; } = string.Empty;
        [Required] public DateTime BirthDate { get; set; }
        [Required] public string Gender { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
