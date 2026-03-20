using ClinicManagement.DTO.TimeSlot;
using ClinicManagement.Models;
using System.Reflection;
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

        public async Task<APIResponse<TimeSlotsDto>> CreateTimeSlot(CreateUpdateTimeSlotDto model)
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

        public async Task<APIResponse<List<TimeSlotsDto>>> GetMyTimeSlots()
        {
            AddToken();

            var response = await _httpClient.GetAsync("api/v1/TimeSlot/DoctorTimeSlots");

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

        public async Task<APIResponse<object>> EditSlot(CreateUpdateTimeSlotDto model,int slotId)
        {

            AddToken();

            var json = JsonSerializer.Serialize(model);

            //var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"api/v1/TimeSlot/{slotId}/update")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);

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

            if(apiResponse == null)
            {
                return new APIResponse<object>
                {
                    IsSuccess = false,
                    DisplayMessage = "Invalid response from server"
                };
            }

            return apiResponse;
        }

        public async Task<APIResponse<TimeSlotsDto>> GetTimeSlotById(int slotId)
        {
            AddToken();

            var response = await _httpClient.GetAsync($"api/v1/TimeSlot/{slotId}");

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new APIResponse<TimeSlotsDto>
                {
                    IsSuccess = false,
                    DisplayMessage = "Server error. Please try again."
                };
            }

            var apiResponse = JsonSerializer.Deserialize<APIResponse<TimeSlotsDto>>(
                responseContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                });

            if (apiResponse == null)
            {
                return new APIResponse<TimeSlotsDto>
                {
                    IsSuccess = false,
                    DisplayMessage = "Invalid response from server"
                };
            }

            return apiResponse;
        }

        public async Task<APIResponse<object>> DeleteSlotId(int slotId)
        {
            AddToken();

            var response = await _httpClient.DeleteAsync($"api/v1/TimeSlot/{slotId}");

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


    }
}
