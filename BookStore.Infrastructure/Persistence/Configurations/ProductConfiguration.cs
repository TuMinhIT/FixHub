using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixHub.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
            builder.Property(p => p.Sku).IsRequired().HasMaxLength(100);
            builder.HasIndex(p => p.Sku).IsUnique();
            builder.Property(p => p.Price).HasColumnType("numeric(18,2)");
            builder.Property(p => p.CapacityHp).HasColumnType("numeric(5,2)");
            builder.Property(x => x.Description)
                                    .HasColumnType("text");
            builder.Property(x => x.SpecificationsJson).HasColumnType("jsonb");
            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.Images)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
