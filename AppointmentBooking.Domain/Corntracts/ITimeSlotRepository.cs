using AppointmentBooking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Domain.Corntracts
{
    public interface ITimeSlotRepository : IGenericRepository<TimeSlot>
    {
        Task UpdateAsync(TimeSlot timeSlot);
        Task<bool> ExistsAsync(Expression<Func<TimeSlot, bool>> predicate);
        Task<IEnumerable<TimeSlot>> GetAllTimeSlotsAsync();

    }
}
