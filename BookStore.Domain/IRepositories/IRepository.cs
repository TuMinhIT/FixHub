
namespace BookStore.Domain.IRepositories
{
   public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> Find(Func<T, bool> predicate);
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);

        //Task SaveChangesAsync(CancellationToken cancellation = default);
    }
}
