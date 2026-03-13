using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Models;
using AppointmentBooking.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure.Repositories
{
    public class TimeSlotRepository : GenericRepository<TimeSlot>, ITimeSlotRepository
    {
        public TimeSlotRepository(AppointmentDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<bool> ExistsAsync(Expression<Func<TimeSlot, bool>> predicate)
        {
            return await _dbContext.TimeSlots.AnyAsync(predicate);
        }

        public async Task<IEnumerable<TimeSlot>> GetAllTimeSlotsAsync()
        {
            return await _dbContext.TimeSlots.Include(ts => ts.Doctor).ToListAsync();
        }

        public async Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync()
        {
            var now = DateTime.UtcNow;
            var today = DateOnly.FromDateTime(now);
            var currentTime = now.TimeOfDay;

            return await _dbContext.TimeSlots
                .Include(ts => ts.Doctor)
                .Where(ts =>                   
                        ts.SlotDate > today ||

                        (ts.SlotDate == today && ts.StartTime > currentTime)                    
                )
                .OrderBy(ts => ts.SlotDate)
                .ThenBy(ts => ts.StartTime)
                .ToListAsync();
        }

        public async Task<TimeSlot> GetTimeSlotsWithDoctorDetailsAsync(Expression<Func<TimeSlot, bool>> filter)
        {
            return await _dbContext.TimeSlots.Include(x => x.Doctor).FirstOrDefaultAsync(filter);
        }

        public async Task<IEnumerable<TimeSlot>> GetTimeSlotsByDoctorIdAsync(int doctorId)
        {
            return await _dbContext.TimeSlots.Where(ts => ts.DoctorId == doctorId).ToListAsync();
        }

        public async Task UpdateAsync(TimeSlot timeSlot)
        {
            _dbContext.Update(timeSlot);
            await _dbContext.SaveChangesAsync();
        }
    }
}
