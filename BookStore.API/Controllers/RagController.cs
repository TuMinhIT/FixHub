using FixHub.Application.Common.Models;
using FixHub.Application.Features.Rag.Commands.AskRag;
using FixHub.Application.Features.Rag.Commands.ReindexAll;
using FixHub.Application.Features.Rag.Commands.SubmitFeedback;
using FixHub.Application.Features.Rag.DTOs;
using FixHub.Application.Features.Rag.Queries.SearchRag;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FixHub.API.Controllers;

[ApiController]
[Route("api/rag")]
[Route("api/v1/rag")]
[EnableRateLimiting("rag")]
public sealed class RagController : ControllerBase
{
    private readonly IMediator _mediator;

    public RagController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromBody] RagSearchRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new SearchRagQuery(request.Query, request.TopK), cancellationToken);
        return Ok(new ApiResponse<RagSearchResponse>(response));
    }

    [HttpPost("ask")]
    [AllowAnonymous]
    public async Task<IActionResult> Ask([FromBody] RagAskRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new AskRagCommand(request.Question, request.ProductId, request.TopK),
            cancellationToken);
        return Ok(new ApiResponse<RagAnswerResponse>(response));
    }

    [HttpPost("feedback")]
    [AllowAnonymous]
    public async Task<IActionResult> Feedback([FromBody] RagFeedbackRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new SubmitFeedbackCommand(request.Question, request.WasHelpful, request.Comment),
            cancellationToken);
        return Ok(new ApiResponse<Guid>(id, "Feedback submitted"));
    }

    [HttpPost("reindex")]
    [HttpPost("/api/v1/admin/rag/reindex")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reindex(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ReindexAllCommand(), cancellationToken);
        return Ok(new ApiResponse<bool>(response, "Knowledge base reindexed successfully"));
    }
}

public sealed class RagAskRequest
{
    public string Question { get; set; } = string.Empty;
    public Guid? ProductId { get; set; }
    public int TopK { get; set; } = 5;
}

public sealed class RagSearchRequest
{
    public string Query { get; set; } = string.Empty;
    public int TopK { get; set; } = 5;
}

public sealed class RagFeedbackRequest
{
    public string Question { get; set; } = string.Empty;
    public bool WasHelpful { get; set; }
    public string? Comment { get; set; }
}
