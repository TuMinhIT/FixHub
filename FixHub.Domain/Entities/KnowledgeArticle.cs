
namespace FixHub.Domain.Entities
{
    public class KnowledgeArticle
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Tags { get; set; }
        public string Status { get; set; } = KnowledgeArticleStatuses.Published;
        public string? Source { get; set; }
        public int Version { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }    

        public ICollection<KnowledgeChunk> Chunks { get; set; } = new List<KnowledgeChunk>();
    }
}
