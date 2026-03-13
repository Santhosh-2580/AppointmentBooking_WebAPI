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
        Task<IEnumerable<TimeSlot>> GetTimeSlotsByDoctorIdAsync(int doctorId);
        Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync();
        Task<TimeSlot> GetTimeSlotsWithDoctorDetailsAsync(Expression<Func<TimeSlot, bool>> filter);

    }
}
