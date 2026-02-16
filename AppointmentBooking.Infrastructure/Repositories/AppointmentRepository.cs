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

        public async Task<IEnumerable<Appointment>> GetAppointmentDetailsAsync()
        {
            return await _dbContext.Appointments
                        .Include(a => a.Patient)
                        .Include(a => a.TimeSlot)
                        .ThenInclude(ts => ts.Doctor)
                        .Where(a => a.Status == AppointmentStatus.Booked)
                        .ToListAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _dbContext.Update(appointment);
            await _dbContext.SaveChangesAsync();
        }
    }
}
