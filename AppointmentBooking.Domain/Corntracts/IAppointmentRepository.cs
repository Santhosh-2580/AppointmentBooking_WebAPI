using AppointmentBooking.Domain.Enums;
using AppointmentBooking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Domain.Corntracts
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task UpdateAsync(Appointment appointment);
        IQueryable<Appointment> GetAll();
        Task<Appointment> GetAppointmentDetailsByIdAsync(int appointmentId);
        Task<IEnumerable<Appointment>> GetAllAppointmentDetailsforAdminAsync();
        Task<IEnumerable<Appointment>> GetAllAppointmentDetailsforPatientAsync(int patientId);
        Task<IEnumerable<Appointment>> GetAllAppointmentDetailsforDoctorAsync(int doctorId);
        Task<bool> ExistsAsync(Expression<Func<Appointment, bool>> predicate);            
        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateOnly date);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsforPatientDashboard(int patientId);


    }
}
