using FixHub.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FixHub.Infrastructure.Persistence.Repositories
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
            return _dbSet.AsNoTracking();
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            var items = entities.ToList();
            await _dbSet.AddRangeAsync(items);
            return items;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var key = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.SingleOrDefault();
            if (key == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} has no single primary key.");

            var keyValue = key.PropertyInfo?.GetValue(entity);
            var existingEntity = await _dbSet.FindAsync(new[] { keyValue });
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

        public async Task<T?> FindById(Guid id)
        {
            var result = await _dbSet.FindAsync(id);
            return result;
        }
    }
}
