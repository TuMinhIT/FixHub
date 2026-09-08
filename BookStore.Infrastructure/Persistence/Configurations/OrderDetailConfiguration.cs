using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixHub.Infrastructure.Persistence.Configurations
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.HasKey(od => od.Id);
            builder.Property(od => od.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Property(od => od.Subtotal).HasColumnType("decimal(18,2)");
            builder.Property(od => od.ProductNameSnapshot).HasMaxLength(255);
            builder.Property(od => od.SkuSnapshot).HasMaxLength(100);
            builder.Property(od => od.ServiceNameSnapshot).HasMaxLength(255);
            
            builder.HasOne(od => od.Order)
                   .WithMany(o => o.OrderDetails)
                   .HasForeignKey(od => od.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(od => od.Product)
                   .WithMany()
                   .HasForeignKey(od => od.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(od => od.RepairService)
                   .WithMany()
                   .HasForeignKey(od => od.RepairServiceId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
