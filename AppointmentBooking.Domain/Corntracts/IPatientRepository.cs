using AppointmentBooking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Domain.Corntracts
{
    public interface IPatientRepository : IGenericRepository<Patient>
    {
            Task UpdateAsync(Patient patient);
    }
}
