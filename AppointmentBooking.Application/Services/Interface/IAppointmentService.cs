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
        Task<AppointmentsDto> CreateAppointmentAsync(CreateAppointmentDto appointmentDto,string userId);
        Task<List<AppointmentsDto>> GetAllAppointmentsOfUsersAsync(string userId, string role);
        Task<List<AppointmentsDto>> GetAppointmentsForUserDashboardAsync(string userId, string role);
        Task CancelAppointmentAsync(string userId, int AppointmentId);
        Task RescheduleAppointmentAsync(string userId, RescheduleAppointmentDto dto);
        Task MarkasCompletedAsync(int AppointmentId);

    }
}
