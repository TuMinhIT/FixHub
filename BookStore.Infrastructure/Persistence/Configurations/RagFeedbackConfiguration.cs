using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixHub.Infrastructure.Persistence.Configurations;

public class RagFeedbackConfiguration : IEntityTypeConfiguration<RagFeedback>
{
    public void Configure(EntityTypeBuilder<RagFeedback> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Question).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.Comment).HasMaxLength(2000);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
