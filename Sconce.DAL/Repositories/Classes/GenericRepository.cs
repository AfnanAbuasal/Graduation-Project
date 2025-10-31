using Microsoft.EntityFrameworkCore;
using Sconce.DAL.Data;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Classes
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool withTracking = false)
        {
            if (withTracking)
                return await _context.Set<T>().ToListAsync();

            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int Id)
            => await _context.Set<T>().FindAsync(Id);

        public async Task<int> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return await _context.SaveChangesAsync();
        }
    }
}
