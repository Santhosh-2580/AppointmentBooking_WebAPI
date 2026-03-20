using AppointmentBooking.Application.Helper;
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Enums;
using AppointmentBooking.Domain.Models;
using AppointmentBooking.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(AppointmentDbContext dbContext) : base(dbContext)
        {
            
        }

        public async Task<bool> ExistsAsync(Expression<Func<Appointment, bool>> predicate)
        {
            return await _dbContext.Appointments.AnyAsync(predicate);
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentDetailsforAdminAsync()
        {
            return await _dbContext.Appointments
                        .Include(a => a.Patient)
                        .Include(a => a.TimeSlot)
                        .ThenInclude(ts => ts.Doctor)                        
                        .ToListAsync();
        }        

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateOnly date)
        {
            return await _dbContext.Appointments
                 .Include(a => a.Patient)
                 .Include(a => a.TimeSlot)
                 .ThenInclude(ts => ts.Doctor)
                 .Where(a => a.TimeSlot.SlotDate == date)
                 .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsByDoctorIdAsync(int doctorId)
        {
            return await _dbContext.Appointments
                .AsNoTracking()
                        .Include(a => a.Patient)
                        .Include(a => a.TimeSlot)
                        .ThenInclude(ts => ts.Doctor)
                        .Where(a => a.TimeSlot.DoctorId == doctorId && a.Status == AppointmentStatus.Booked && a.TimeSlot.SlotDate >= DateOnly.FromDateTime(DateTime.Today))
                        .ToListAsync();
        }       

        public async Task UpdateAsync(Appointment appointment)
        {
            _dbContext.Update(appointment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsforPatientDashboard(int patientId)
        {
            return await _dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.TimeSlot)
                .ThenInclude(ts => ts.Doctor)
                .Where(a => a.PatientId == patientId && a.Status == AppointmentStatus.Booked && a.TimeSlot.SlotDate >= DateOnly.FromDateTime(DateTime.Today))
                .OrderBy(a => a.TimeSlot.SlotDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentDetailsforPatientAsync(int patientId)
        {
            return await _dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.TimeSlot)
                .ThenInclude(ts => ts.Doctor)
                .Where(a => a.PatientId == patientId)
                .OrderBy(a => a.TimeSlot.SlotDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentDetailsforDoctorAsync(int doctorId)
        {
            return await _dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.TimeSlot)
                .ThenInclude(ts => ts.Doctor)
                .Where(a => a.TimeSlot.DoctorId == doctorId)
                .OrderBy(a => a.TimeSlot.SlotDate)
                .ToListAsync();
        }

        public async Task<Appointment> GetAppointmentDetailsByIdAsync(int appointmentId)
        {
            return await _dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.TimeSlot)
                    .ThenInclude(ts => ts.Doctor)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }
    }
}
