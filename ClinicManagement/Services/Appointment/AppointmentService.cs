using ClinicManagement.DTO.Appointment;
using ClinicManagement.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClinicManagement.Services.Appointment
{
    public class AppointmentService : BaseApiService
    {
        public AppointmentService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor)
        {

        }

        public async Task<List<AppointmentsDto>> GetMyAppointmentsasync()
        {
            AddToken();
            var response = await _httpClient.GetAsync("api/v1/Appointment/MyAppointments");

            var json = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<APIResponse<List<AppointmentsDto>>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return apiResponse?.Result ?? new List<AppointmentsDto>();


        }
    }
}
