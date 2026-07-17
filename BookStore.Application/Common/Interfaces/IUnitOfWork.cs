

using BookStore.Domain.IRepositories;

namespace BookStore.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IBookRepository BookRepository { get; } 
       

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
