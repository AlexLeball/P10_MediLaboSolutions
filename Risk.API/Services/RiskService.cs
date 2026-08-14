using Risk.API.DTOs;
using Risk.API.Enums;
using Risk.API.Interfaces;
using System.Net.Http.Headers;

namespace Risk.API.Services
{
    public class RiskService : IRiskService
    {
        // 12 termes déclencheurs — chacun compté au plus une fois
        private static readonly string[] TriggerTerms =
        [
            "Hémoglobine A1C",
            "Microalbumine",
            "Taille",
            "Poids",
            "Fumeur",
            "Fumeuse",
            "Anormal",
            "Cholestérol",
            "Vertiges",
            "Rechute",
            "Réaction",
            "Anticorps"
        ];

        private readonly IHttpClientFactory _httpClientFactory;

        public RiskService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Méthode principale pour générer le rapport de risque 
        public async Task<RiskReportDto?> GetRiskReportAsync(int patientId, string bearerToken)
        {
            var patient = await GetPatientAsync(patientId, bearerToken);
            if (patient is null)
                return null;

            var notes = await GetNotesAsync(patientId, bearerToken);

            var age = CalculateAge(patient.BirthDate); 
            var isMale = patient.Gender.StartsWith("M", StringComparison.OrdinalIgnoreCase);
            var triggerCount = CountTriggers(notes);
            var riskLevel = DetermineRiskLevel(age, isMale, triggerCount);

            return new RiskReportDto
            {
                PatientId = patient.Id,
                FullName = $"{patient.FirstName} {patient.LastName}",
                Age = age,
                TriggerCount = triggerCount,
                RiskLevel = GetRiskLevelDisplay(riskLevel)
            };
        }

        // Appels aux APIs amont

        private async Task<PatientDto?> GetPatientAsync(int patientId, string bearerToken)
        {
            var client = _httpClientFactory.CreateClient("PatientApi");
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/patient/{patientId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<PatientDto>();
        }

        private async Task<List<MedicalNoteDto>> GetNotesAsync(int patientId, string bearerToken)
        {
            var client = _httpClientFactory.CreateClient("NotesApi");
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/notes/{patientId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return [];

            return await response.Content.ReadFromJsonAsync<List<MedicalNoteDto>>() ?? [];
        }

        // Comptage des déclencheurs 

        private static int CountTriggers(List<MedicalNoteDto> notes)
        {
            if (notes.Count == 0)
                return 0;

            // Concaténer toutes les notes en un seul texte
            var combinedText = string.Join(" ", notes.Select(n => n.Content));

            // Chaque terme ne compte qu'une fois, quelle que soit la fréquence
            return TriggerTerms.Count(term =>
                combinedText.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        // Calcul de l'âge 

        private static int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age))
                age--;
            return age;
        }

        //Règles de risque

        private static RiskLevel DetermineRiskLevel(int age, bool isMale, int triggerCount)
        {
            if (triggerCount == 0)
                return RiskLevel.None;

            if (age >= 30)
            {
                if (triggerCount >= 8) return RiskLevel.EarlyOnset;
                if (triggerCount is 6 or 7) return RiskLevel.InDanger;
                if (triggerCount >= 2) return RiskLevel.Borderline;
                return RiskLevel.None;
            }

            // age < 30 
            if (isMale)
            {
                if (triggerCount >= 5) return RiskLevel.EarlyOnset;
                if (triggerCount is 3 or 4) return RiskLevel.InDanger;
                return RiskLevel.None;
            }

            // Féminin, age < 30
            if (triggerCount >= 7) return RiskLevel.EarlyOnset;
            if (triggerCount is >= 4 and <= 6) return RiskLevel.InDanger;
            return RiskLevel.None;
        }

        private static string GetRiskLevelDisplay(RiskLevel level) => level switch
        {
            RiskLevel.None => "None",
            RiskLevel.Borderline => "Borderline",
            RiskLevel.InDanger => "In Danger",
            RiskLevel.EarlyOnset => "Early Onset",
            _ => "Unknown"
        };
    }
}