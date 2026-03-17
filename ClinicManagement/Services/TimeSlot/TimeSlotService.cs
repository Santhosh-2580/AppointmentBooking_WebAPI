using ClinicManagement.DTO.TimeSlot;
using ClinicManagement.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClinicManagement.Services.TimeSlot
{
    public class TimeSlotService : BaseApiService
    {
        public TimeSlotService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor) { }
        
        public async Task<APIResponse<List<TimeSlotsDto>>> GetAvailableSlots()
        {
            AddToken();

            var response = await _httpClient.GetAsync("api/v1/TimeSlot/AvailableSlots");

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new APIResponse<List<TimeSlotsDto>>
                {
                    IsSuccess = false,
                    DisplayMessage = "Server error. Please try again."
                };
            }

            var apiResponse = JsonSerializer.Deserialize<APIResponse<List<TimeSlotsDto>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return apiResponse;
        }

        public async Task<APIResponse<TimeSlotsDto>> CreateTimeSlot(CreateTimeSlotDto model)
        {
            AddToken();

            var json = JsonSerializer.Serialize(model);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/v1/TimeSlot/Add", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new APIResponse<TimeSlotsDto>
                {
                    IsSuccess = false,
                    DisplayMessage = "Server error. Please try again."
                };
            }

            var apiResponse = JsonSerializer.Deserialize<APIResponse<TimeSlotsDto>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return apiResponse;

        }


    }
}
