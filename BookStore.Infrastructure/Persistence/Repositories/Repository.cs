using BookStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Infrastructure.Persistence.Repositories
{
    public class Repository<T> : IRepository<T>
     where T : class
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(AppDbContext context )
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }


        public async Task<IQueryable<T>> Find(Func<T, bool> predicate)
        {
            return _dbSet.Where(predicate).AsQueryable();
        }

        public Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var existingEntity = await _dbSet.FindAsync(entity);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                throw new ArgumentException("Entity not found in the database.");
            }
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                return true;
            }
            return false;
        }


        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return  entity;
            
        }
    }
}
