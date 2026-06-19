using System.ComponentModel.DataAnnotations;

namespace Frontend.Web.Models
{
    public class PatientDto
    {
        // data transfer object for patient information
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [Range(typeof(DateTime), "1900-01-01", "9999-12-31",
            ErrorMessage = "Birth date must be between 1900-01-01 and yesterday.")]
        public DateTime BirthDate { get; set; }

        public string Gender { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PractitionerId { get; set; }
    }
}

