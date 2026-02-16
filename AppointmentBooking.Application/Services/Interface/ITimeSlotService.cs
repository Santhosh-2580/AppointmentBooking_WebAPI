using AppointmentBooking.Application.DTO.TimeSlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Services.Interface
{
    public interface ITimeSlotService
    {
        Task<TimeSlotsDto> GetTimeSlotByIdAsync(int id);
        Task<IEnumerable<TimeSlotsDto>> GetTimeSlotsAsync();
        Task<IEnumerable<TimeSlotsDto>> GetTimeSlotsByFilterAsync(int? doctorId);
        Task<CreateTimeSlotDto> CreateTimeSlotAsync(CreateTimeSlotDto timeSlotDto);
        Task UpdateTimeSlotAsync(int id, UpdateTimeSlotDto updateTimeSlotDto);
        Task DeleteTimeSlotAsync(int id);
    }
}
