using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixHub.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new
            {
                x.UserId,
                x.IdempotencyKey
            })
            .IsUnique()
            .HasFilter("\"IdempotencyKey\" IS NOT NULL");

            builder.Property(x => x.IdempotencyKey).HasMaxLength(255);
        }
    }
}
