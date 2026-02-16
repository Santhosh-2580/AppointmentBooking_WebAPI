using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Models;
using AppointmentBooking.Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure.Repositories
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(AppointmentDbContext dbContext) : base(dbContext)
        {

        }

        public async Task UpdateAsync(Doctor doctor)
        {
            _dbContext.Update(doctor);
            await _dbContext.SaveChangesAsync();
        }
    }
}
