using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Frontend.Web.Models;

namespace Frontend.Web.Services
{
    public class PatientApiService
    {
        private readonly HttpClient _http;

        public PatientApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var response = await _http.PostAsync(
                "/api/auth/login",
                new StringContent(
                    JsonSerializer.Serialize(new { email, password }),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement.GetProperty("token").GetString();
        }

        public async Task<List<PatientDto>?> GetPatientsAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/patient");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<PatientDto>>();
        }

        public async Task<PatientDto?> GetPatientByIdAsync(int id, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/patient/{id}");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<PatientDto>();
        }

        public async Task<bool> CreatePatientAsync(PatientDto dto, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/patient");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task UpdatePatientAsync(PatientDto dto, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            await _http.PutAsJsonAsync($"/api/patient/{dto.Id}", dto);
        }

        public async Task AddNoteAsync(MedicalNoteDto dto, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/notes");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json"
            );

            await _http.SendAsync(request);
        }
        public async Task<List<MedicalNoteDto>> GetNotesAsync(int patientId, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/notes/{patientId}");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            return await response.Content.ReadFromJsonAsync<List<MedicalNoteDto>>();
        }

        public async Task<(bool Success, string? Error)> RegisterAsync(
            string email, string password, string role, string fullName, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/register");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(
                JsonSerializer.Serialize(new { email, password, role, fullName }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _http.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await response.Content.ReadAsStringAsync());
        }

        public async Task<List<PractitionerDto>> GetPractitionersAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/practitioners");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return new List<PractitionerDto>();

            return await response.Content.ReadFromJsonAsync<List<PractitionerDto>>()
                   ?? new List<PractitionerDto>();
        }

        public async Task<List<UserDto>> GetUsersAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/users");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return new List<UserDto>();

            return await response.Content.ReadFromJsonAsync<List<UserDto>>()
                   ?? new List<UserDto>();
        }

        public async Task<UserDto?> GetUserByIdAsync(string id, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/auth/users/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UserDto>();
        }

        public async Task<(bool Success, string? Error)> UpdateUserAsync(
            EditUserViewModel model, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/auth/users/{model.Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    model.FullName,
                    model.Email,
                    model.Role,
                    NewPassword = model.NewPassword
                }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _http.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await response.Content.ReadAsStringAsync());
        }
    }
}
