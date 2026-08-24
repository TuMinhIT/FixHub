using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixHub.Infrastructure.Persistence.Configurations
{
    public class RepairServiceConfiguration : IEntityTypeConfiguration<RepairService>
    {
        public void Configure(EntityTypeBuilder<RepairService> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(255);
            builder.Property(s => s.BasePrice).HasColumnType("decimal(18,2)");
            
            builder.HasOne(s => s.Category)
                   .WithMany(c => c.RepairServices)
                   .HasForeignKey(s => s.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
