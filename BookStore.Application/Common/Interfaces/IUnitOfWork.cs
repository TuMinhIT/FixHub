

using FixHub.Domain.IRepositories;

namespace FixHub.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }

        IBookRepository BookRepository { get; }

        IRefreshTokenRepository RefreshTokenRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
     
    }
}
