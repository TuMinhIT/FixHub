using AutoMapper;
using FixHub.Application.Features.KnowledgeArticles.Commands.CreateKnowledgeArticle;
using FixHub.Application.Features.KnowledgeArticles.Commands.UpdateKnowledgeArticle;
using FixHub.Application.Features.KnowledgeArticles.DTOs;
using FixHub.Domain.Entities;

namespace FixHub.Application.Common.Mappings
{
    public class KnowledgeArticleMapping : Profile
    {
        public KnowledgeArticleMapping()
        {
            CreateMap<KnowledgeArticle, KnowledgeArticleResponse>();
            CreateMap<CreateKnowledgeArticleCommand, KnowledgeArticle>();
            CreateMap<UpdateKnowledgeArticleCommand, KnowledgeArticle>();
        }
    }
}
