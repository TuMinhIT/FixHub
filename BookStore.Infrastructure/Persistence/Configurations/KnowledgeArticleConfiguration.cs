using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixHub.Infrastructure.Persistence.Configurations
{
    public class KnowledgeArticleConfiguration : IEntityTypeConfiguration<KnowledgeArticle>
    {
        public void Configure(EntityTypeBuilder<KnowledgeArticle> builder)
        {
            builder.HasKey(k => k.Id);
            builder.Property(k => k.Title).IsRequired().HasMaxLength(255);
            builder.Property(k => k.Content).IsRequired().HasColumnType("text");
            builder.Property(k => k.Status).IsRequired().HasMaxLength(50)
                .HasDefaultValue(KnowledgeArticleStatuses.Published);
            builder.Property(k => k.Source).HasMaxLength(500);
            builder.Property(k => k.Version).HasDefaultValue(1);
            builder.HasMany(k => k.Chunks)
                .WithOne(c => c.Article)
                .HasForeignKey(c => c.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
