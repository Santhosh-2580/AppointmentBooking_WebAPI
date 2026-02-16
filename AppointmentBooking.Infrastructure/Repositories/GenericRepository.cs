using AppointmentBooking.Domain.Common;
using AppointmentBooking.Domain.Corntracts;
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
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        protected readonly AppointmentDbContext _dbContext;

        public GenericRepository(AppointmentDbContext dbContext)
        {
            _dbContext  = dbContext;
        }

        public async Task<T> CreateAsync(T entity)
        {
           var addEntity = await _dbContext.Set<T>().AddAsync(entity);
           await _dbContext.SaveChangesAsync();
           return addEntity.Entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Expression<Func<T, bool>> condition)
        {
            return await _dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(condition);
        }
    }
}
