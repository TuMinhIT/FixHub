using System;
using Pgvector;

namespace FixHub.Domain.Entities
{
    public class KnowledgeArticle
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Tags { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The vector embedding used for semantic search (RAG)
        /// </summary>
        public Vector? Embedding { get; set; }
    }
}
