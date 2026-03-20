using ClinicManagement.DTO.Appointment;
using ClinicManagement.DTO.TimeSlot;
using ClinicManagement.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClinicManagement.Services.Appointment
{
    public class AppointmentService : BaseApiService
    {
        public AppointmentService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor)
        {

        }

        public async Task<APIResponse<List<AppointmentsDto>>> GetMyAppointmentsasync()
        {
            AddToken();
            var response = await _httpClient.GetAsync("api/v1/Appointment/upcomingAppointments");

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new APIResponse<List<AppointmentsDto>>
                {
                    IsSuccess = false,
                    DisplayMessage = "Server error. Please try again."
                };
            }

            var apiResponse = JsonSerializer.Deserialize<APIResponse<List<AppointmentsDto>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return apiResponse;

        }

        public async Task<APIResponse<AppointmentsDto>> CreateAppointment(CreateAppointmentDto model)
        {
            AddToken();

            var json = JsonSerializer.Serialize(model);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/v1/Appointment/Create", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new APIResponse<AppointmentsDto>
                {
                    IsSuccess = false,
                    DisplayMessage = "Server error. Please try again."
                };
            }

            var apiResponse = JsonSerializer.Deserialize<APIResponse<AppointmentsDto>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return apiResponse;

        }

        public async Task<APIResponse<object>> MarkAsCompletedAsync(int appointmentId)
        {
            AddToken();

            var response = await _httpClient.PatchAsync($"api/v1/Appointment/{appointmentId}/Complete",null);

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
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                });

            if (apiResponse == null)
            {
                return new APIResponse<object>
                {
                    IsSuccess = false,
                    DisplayMessage = "Invalid response from server"
                };
            }

            return apiResponse;
        }

        public async Task<APIResponse<object>> MarkAsCancelAsync(int appointmentId)
        {
            AddToken();

            var response = await _httpClient.PatchAsync($"api/v1/Appointment/{appointmentId}/Cancel", null);

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
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                });

            if (apiResponse == null)
            {
                return new APIResponse<object>
                {
                    IsSuccess = false,
                    DisplayMessage = "Invalid response from server"
                };
            }

            return apiResponse;
        }

        public async Task<APIResponse<object>> RescheduleAppointment(RescheduleAppointmentDto model)
        {
            AddToken();

            var json = JsonSerializer.Serialize(model);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync("api/v1/Appointment/reschedule", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new APIResponse<object>
                {
                    IsSuccess = false,
                    DisplayMessage = "Server error. Please try again."
                };
            }

            var apiResponse = JsonSerializer.Deserialize<APIResponse<object>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return apiResponse;

        }

        public async Task<APIResponse<List<AppointmentsDto>>> GetAllAppointmentDetailsAsync()
        {
            AddToken();
            var response = await _httpClient.GetAsync("api/v1/Appointment/UsersAppointments");

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new APIResponse<List<AppointmentsDto>>
                {
                    IsSuccess = false,
                    DisplayMessage = "Server error. Please try again."
                };
            }

            var apiResponse = JsonSerializer.Deserialize<APIResponse<List<AppointmentsDto>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return apiResponse;

        }

        public async Task<APIResponse<AppointmentsDto>> GetAppointmentByIdAsync(int appointmentId)
        {
            AddToken();

            var response = await _httpClient.GetAsync($"api/v1/Appointment/{appointmentId}/AppointmentDetailsById");

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new APIResponse<AppointmentsDto>
                {
                    IsSuccess = false,
                    DisplayMessage = "Server error. Please try again."
                };
            }

            var apiResponse = JsonSerializer.Deserialize<APIResponse<AppointmentsDto>>(
                responseContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                });

            if (apiResponse == null)
            {
                return new APIResponse<AppointmentsDto>
                {
                    IsSuccess = false,
                    DisplayMessage = "Invalid response from server"
                };
            }

            return apiResponse;
        }

    }
}
