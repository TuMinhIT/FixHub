using FixHub.Application.Common.Interfaces.Rag;
using FixHub.Application.Features.Rag.DTOs;
using MediatR;

namespace FixHub.Application.Features.Rag.Queries.SearchRag;

public sealed class SearchRagHandler : IRequestHandler<SearchRagQuery, RagSearchResponse>
{
    private readonly IRagSearchService _searchService;

    public SearchRagHandler(IRagSearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<RagSearchResponse> Handle(SearchRagQuery request, CancellationToken cancellationToken)
    {
        return new RagSearchResponse
        {
            Sources = await _searchService.SearchAsync(request.Query, request.TopK, cancellationToken)
        };
    }
}
