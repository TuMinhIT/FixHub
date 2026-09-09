using FixHub.Application.Features.Rag.DTOs;
using MediatR;

namespace FixHub.Application.Features.Rag.Commands.AskRag;

public sealed record AskRagCommand(string Question, Guid? ProductId = null, int TopK = 5) : IRequest<RagAnswerResponse>;
