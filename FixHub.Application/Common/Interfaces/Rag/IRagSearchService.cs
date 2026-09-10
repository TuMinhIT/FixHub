using FixHub.Application.Features.Rag.DTOs;

namespace FixHub.Application.Common.Interfaces.Rag;

public interface IRagSearchService
{
    Task<IReadOnlyList<RagSourceResponse>> SearchAsync(
        string question,
        int topK = 5,
        CancellationToken cancellationToken = default);
}
