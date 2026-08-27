namespace FixHub.Application.Features.KnowledgeArticles.DTOs
{
    public class KnowledgeArticleResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Tags { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
