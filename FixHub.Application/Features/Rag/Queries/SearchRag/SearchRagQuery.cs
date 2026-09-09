using FixHub.Application.Features.Rag.DTOs;
using MediatR;

namespace FixHub.Application.Features.Rag.Queries.SearchRag;

public sealed record SearchRagQuery(string Query, int TopK = 5) : IRequest<RagSearchResponse>;
