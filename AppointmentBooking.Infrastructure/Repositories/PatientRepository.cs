#nullable enable
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Models;
using AppointmentBooking.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure.Repositories
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public PatientRepository(AppointmentDbContext dbContext) : base(dbContext)
        {
            
        }

        public async Task<Patient?> GetByUserIdAsync(string userId)
        {
          return await _dbContext.Patients.FirstOrDefaultAsync(p => p.UserId == userId);            
        }

        public async Task UpdateAsync(Patient patient)
        {
            _dbContext.Update(patient);
            await _dbContext.SaveChangesAsync();
        }
    }
}
