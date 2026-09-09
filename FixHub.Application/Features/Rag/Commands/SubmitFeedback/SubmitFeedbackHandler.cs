using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using MediatR;

namespace FixHub.Application.Features.Rag.Commands.SubmitFeedback;

public sealed class SubmitFeedbackHandler : IRequestHandler<SubmitFeedbackCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public SubmitFeedbackHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(SubmitFeedbackCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question) || request.Question.Length > 2000)
            throw new BadRequestException("Question is required and must not exceed 2000 characters.");
        if (request.Comment?.Length > 2000)
            throw new BadRequestException("Comment must not exceed 2000 characters.");

        var feedback = new RagFeedback
        {
            UserId = _currentUserService.UserIdOrNull,
            Question = request.Question.Trim(),
            WasHelpful = request.WasHelpful,
            Comment = request.Comment?.Trim()
        };
        await _unitOfWork.RagFeedbackRepository.AddAsync(feedback);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return feedback.Id;
    }
}
