using ClinicManagement.Models;
using ClinicManagement.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace ClinicManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<APIResponse<LoginResponse>> Login(Login model)
        {
            var json = JsonSerializer.Serialize(model);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/v1/User/Login", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new APIResponse<LoginResponse>
                {
                    IsSuccess = false,
                    DisplayMessage = "Server error. Please try again."
                };
            }

            var apiResponse = JsonSerializer.Deserialize<APIResponse<LoginResponse>>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return apiResponse;
        }

        public async Task<APIResponse<object>> Register(Register model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/v1/User/Register-patient", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new APIResponse<object>
                {
                    IsSuccess = false,
                    DisplayMessage = "Server error. Please try again."
                };
            }

            var apiResponse = JsonSerializer.Deserialize<APIResponse<object>>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return apiResponse;
        }
    }
}
