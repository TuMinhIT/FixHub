using MediatR;

namespace FixHub.Application.Features.Rag.Commands.ReindexAll;

public sealed record ReindexAllCommand : IRequest<bool>;
