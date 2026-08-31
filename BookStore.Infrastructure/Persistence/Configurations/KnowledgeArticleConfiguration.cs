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
            
            // Assuming 768 dimensions for embedding (can be 1536 for OpenAI, 768 for some open source models)
            //builder.Property(k => k.Embedding)
            //       .HasColumnType("vector(768)");
        }
    }
}
