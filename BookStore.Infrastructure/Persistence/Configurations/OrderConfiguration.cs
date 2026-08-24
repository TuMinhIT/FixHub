using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixHub.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            builder.Property(o => o.Status).IsRequired().HasMaxLength(50);
            
            builder.HasOne(o => o.User)
                   .WithMany()
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Address)
                   .WithMany()
                   .HasForeignKey(o => o.AddressId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
