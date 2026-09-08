using FixHub.Application.Features.Rag.DTOs;

namespace FixHub.Application.Common.Interfaces;

public interface IRagAnswerService
{
    Task<string> GenerateAnswerAsync(
        string question,
        IReadOnlyList<RagSourceResponse> sources,
        CancellationToken cancellationToken = default);
}
