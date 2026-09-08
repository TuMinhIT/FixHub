using MediatR;

namespace FixHub.Application.Features.Rag.Commands.SubmitFeedback;

public sealed record SubmitFeedbackCommand(
    string Question,
    bool WasHelpful,
    string? Comment = null) : IRequest<Guid>;
