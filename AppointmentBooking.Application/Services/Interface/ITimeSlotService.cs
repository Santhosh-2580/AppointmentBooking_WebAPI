using AppointmentBooking.Application.DTO.TimeSlot;
using AppointmentBooking.Domain.Models;
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
        Task<IEnumerable<TimeSlotsDto>> GetAllAvailableTimeSlotsAsync(string userId);
        Task<IEnumerable<TimeSlotsDto>> GetMyTimeSlotsAsync(string userId);
        //Task<IEnumerable<TimeSlotsDto>> GetTimeSlotsByFilterAsync(int? doctorId);
        Task<CreateTimeSlotDto> CreateTimeSlotAsync(CreateTimeSlotDto timeSlotDto, string userId);
        Task UpdateTimeSlotAsync(string userId, UpdateTimeSlotDto updateTimeSlotDto, int timeSlotId);
        Task DeleteTimeSlotAsync(string userId, int id);
    }
}
