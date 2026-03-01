using AppointmentBooking.Application.DTO.Appointment;
using AppointmentBooking.Application.DTO.TimeSlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Services.Interface
{
    public interface IAppointmentService
    {
        Task<AppointmentsDto> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<AppointmentsDto>> GetAllAppointmentsAsync();
        //Task<IEnumerable<AppointmentsDto>> GetAppointmentsByFilterAsync(int? doctorId, int? patientId);
        Task<AppointmentsDto> CreateAppointmentAsync(CreateAppointmentDto appointmentDto,string userId);
        //Task UpdateAppointmentAsync(int id, UpdateAppointmentDto updateAppointmentDto);
        //Task DeleteAppointmentAsync(int id);

        Task<List<AppointmentsDto>> GetAppointmentsForUserAsync(string userId, string role);
        Task CancelAppointmentAsync(string userId, int AppointmentId);
        Task RescheduleAppointmentAsync(string userId, RescheduleAppointmentDto dto);
    }
}
