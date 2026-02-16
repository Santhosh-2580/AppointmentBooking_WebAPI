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
        Task<IEnumerable<Appointment>> GetAppointmentDetailsAsync();
        Task<bool> ExistsAsync(Expression<Func<Appointment, bool>> predicate);

    }
}
