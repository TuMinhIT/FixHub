using BookStore.Domain.Entities;
namespace BookStore.Domain.IRepositories
{
    public interface IUserRepository: IRepository<User>
    {
        // pecific methods for User repository
        // <summary>
        // Get user by email
        // </summary>
        Task<User?> GetByEmailAsync(string email);

        Task<bool> ExistsByEmailAsync(string email);
    }
}
