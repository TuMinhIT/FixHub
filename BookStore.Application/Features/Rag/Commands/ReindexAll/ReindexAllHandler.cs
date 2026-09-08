using FixHub.Application.Common.Interfaces;
using MediatR;

namespace FixHub.Application.Features.Rag.Commands.ReindexAll;

public sealed class ReindexAllHandler : IRequestHandler<ReindexAllCommand, bool>
{
    private readonly IRagIndexingService _indexingService;

    public ReindexAllHandler(IRagIndexingService indexingService)
    {
        _indexingService = indexingService;
    }

    public async Task<bool> Handle(ReindexAllCommand request, CancellationToken cancellationToken)
    {
        await _indexingService.ReindexAllAsync(cancellationToken);
        return true;
    }
}
