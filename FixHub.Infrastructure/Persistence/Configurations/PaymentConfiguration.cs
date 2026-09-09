using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentEntity = FixHub.Domain.Entities.Payment;

namespace FixHub.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<PaymentEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentEntity> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            builder.Property(p => p.InvoiceNumber).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Status).IsRequired();
            builder.Property(p => p.Method).IsRequired();
            builder.Property(p => p.IdempotencyKey).HasMaxLength(255);
            builder.Property(p => p.CheckoutUrl).HasMaxLength(2048);
            //builder.Property(p => p.CheckoutFieldsJson).HasColumnType("nvarchar(max)");

            builder.HasIndex(p => p.IdempotencyKey).IsUnique();
            builder.HasIndex(p => p.OrderId).IsUnique();

            builder.HasOne(p => p.Order)
                   .WithOne(o => o.Payment)
                   .HasForeignKey<PaymentEntity>(p => p.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    
}
