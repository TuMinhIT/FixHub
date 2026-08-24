using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        //public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        //public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        //thì mọi cấu hình sẽ được đưa sang thư mục Configurations.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

    } 
}
