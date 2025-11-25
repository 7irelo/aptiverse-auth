using Aptiverse.Core.Exceptions;
using Aptiverse.Domain.Interfaces;
using Aptiverse.Infrastructure.Data;
using Aptiverse.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Aptiverse.Infrastructure.Repositories
{
    public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context = context;

        public async Task<T> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<T?> GetOneAsync(long id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetManyAsync(Dictionary<string, object>? filters = null)
        {
            var predicate = QueryFilterBuilder<T>.BuildPredicateFromFilters(filters);
            return await _context.Set<T>().Where(predicate).AsNoTracking().ToListAsync();
        }

        public async Task<T> UpdateAsync(long id, T entity)
        {
            var existingEntity = await _context.Set<T>().FindAsync(id) ?? throw new EntityNotFoundException(typeof(T).Name, id);
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(long id)
        {
            await _context.Set<T>().Where(x => EF.Property<long>(x, "Id") == id).ExecuteDeleteAsync();
        }
    }
}
