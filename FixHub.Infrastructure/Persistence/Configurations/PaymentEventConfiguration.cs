using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixHub.Infrastructure.Persistence.Configurations;

public class PaymentEventConfiguration : IEntityTypeConfiguration<PaymentEvent>
{
    public void Configure(EntityTypeBuilder<PaymentEvent> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Provider).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ProviderEventId).IsRequired().HasMaxLength(255);
        builder.Property(x => x.PayloadHash).IsRequired().HasMaxLength(64);
        builder.Property(x => x.PayloadJson).HasColumnType("jsonb");
        builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => new { x.Provider, x.ProviderEventId }).IsUnique();
        builder.HasOne(x => x.Payment)
            .WithMany()
            .HasForeignKey(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
