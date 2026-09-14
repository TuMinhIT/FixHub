using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixHub.Infrastructure.Persistence.Configurations;

public class UploadedImageConfiguration : IEntityTypeConfiguration<UploadedImage>
{
    public void Configure(EntityTypeBuilder<UploadedImage> builder)
    {
        builder.HasKey(image => image.Id);
        builder.Property(image => image.Url).IsRequired().HasMaxLength(1000);
        builder.Property(image => image.PublicId).IsRequired().HasMaxLength(255);
        builder.Property(image => image.FileName).IsRequired().HasMaxLength(255);
        builder.Property(image => image.Format).IsRequired().HasMaxLength(32);
        builder.Property(image => image.SizeBytes).IsRequired();
        builder.Property(image => image.UploadedAt).IsRequired();

        builder.HasIndex(image => image.PublicId).IsUnique();
        builder.HasIndex(image => new { image.UserId, image.UploadedAt });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(image => image.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
