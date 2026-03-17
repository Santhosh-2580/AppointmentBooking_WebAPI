using ClinicManagement.DTO.Patient;
using ClinicManagement.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClinicManagement.Services.Patient
{
    public class PatientService : BaseApiService
    {
        public PatientService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor)
        {
            
        }

        public async Task<APIResponse<PatientDto>> GetMyProfile()
        {
            AddToken();

            var response = await _httpClient.GetAsync("api/v1/Patient/Profile");

            var responseContent = await response.Content.ReadAsStringAsync();

            if(!response.IsSuccessStatusCode)
            {
                return new APIResponse<PatientDto>
                {
                    IsSuccess = false,
                    DisplayMessage = "Server error. Please try again."
                };
            }

            var apiResponse = JsonSerializer.Deserialize<APIResponse<PatientDto>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return apiResponse;
        }

        public async Task<APIResponse<PatientDto>>CreateMyProfile(CreatePatientProfileDto model)
        {
            AddToken();

            var json = JsonSerializer.Serialize(model);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/v1/Patient",content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if(!response.IsSuccessStatusCode)
            {
                return new APIResponse<PatientDto>
                {
                    IsSuccess = false,
                    DisplayMessage = "Server error. Please try again."
                };
            }            

            var apiResponse = JsonSerializer.Deserialize<APIResponse<PatientDto>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return apiResponse;

        }
    }
}
