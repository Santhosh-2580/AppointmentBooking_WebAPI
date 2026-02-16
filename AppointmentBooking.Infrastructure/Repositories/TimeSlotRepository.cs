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

        public async Task UpdateAsync(TimeSlot timeSlot)
        {
            _dbContext.Update(timeSlot);
            await _dbContext.SaveChangesAsync();
        }
    }
}
