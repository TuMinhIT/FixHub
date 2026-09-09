using MediatR;

namespace FixHub.Application.Features.Rag.Commands.ReindexArticle;

public sealed record ReindexArticleCommand(Guid ArticleId) : IRequest<bool>;
