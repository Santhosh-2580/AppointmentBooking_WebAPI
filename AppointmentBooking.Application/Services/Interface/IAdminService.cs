using AppointmentBooking.Application.DTO.Admin;
using AppointmentBooking.Application.DTO.Appointment;
using AppointmentBooking.Application.DTO.Doctor;
using AppointmentBooking.Application.DTO.Patient;
using AppointmentBooking.Application.DTO.TimeSlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Services.Interface
{
    public interface IAdminService
    {
        Task<AdminDashboardDto> GetDashboardAsync();
        Task<TodayDashboardDto> GetTodayDashboardAsync();

        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto> GetPatientByIdAsync(int id);
        Task<bool> DeactivatePatientAsync(int id);

        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto> GetDoctorByIdAsync(int id);
        Task<bool> DeactivateDoctorAsync(int id);

        Task<IEnumerable<TimeSlotsDto>> GetAllTimeSlotsAsync();
        Task<IEnumerable<TimeSlotsDto>> GetTimeSlotsByDoctorAsync(int doctorId);

        Task<IEnumerable<AppointmentsDto>> GetAllAppointmentsAsync();
        Task<IEnumerable<AppointmentsDto>> GetAppointmentsByDateAsync(DateOnly date);
        Task CancelAppointmentByAdminAsync(int appointmentId);

    }
}
