
using BookStore.Domain.Entities;
using BookStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence.Repositories
{

    public class BookRepository :Repository<Book> ,IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context): base(context)
        {
           
        }
  
    }
}
