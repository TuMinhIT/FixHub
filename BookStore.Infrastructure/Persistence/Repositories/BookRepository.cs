
using FixHub.Domain.Entities;
using FixHub.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Infrastructure.Persistence.Repositories
{

    public class BookRepository :Repository<Book> ,IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context): base(context)
        {
           
        }
  
    }
}
