using System;
using System.ComponentModel.DataAnnotations;

namespace Patient.Domain.Entities
{
    public class PatientEntity
    {
        public int Id { get; set; }

        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string LastName { get; set; } = string.Empty;

        [Required]
        [Range(typeof(DateTime), "1900-01-01", "9999-12-31",
            ErrorMessage = "Birth date must be 1900-01-01 or later.")]
        public DateTime BirthDate { get; set; }

        [Required] public string Gender { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
