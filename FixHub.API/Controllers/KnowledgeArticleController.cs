using FixHub.Application.Common.Models;
using FixHub.Application.Features.KnowledgeArticles.Commands.CreateKnowledgeArticle;
using FixHub.Application.Features.KnowledgeArticles.Commands.BulkCreateKnowledgeArticles;
using FixHub.Application.Features.KnowledgeArticles.Commands.DeleteKnowledgeArticle;
using FixHub.Application.Features.KnowledgeArticles.Commands.UpdateKnowledgeArticle;
using FixHub.Application.Features.KnowledgeArticles.DTOs;
using FixHub.Application.Features.KnowledgeArticles.Queries.GetAllKnowledgeArticles;
using FixHub.Application.Features.KnowledgeArticles.Queries.GetKnowledgeArticleById;
using FixHub.Application.Features.Rag.Commands.ReindexArticle;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers
{
    [ApiController]
    [Route("api/v1/knowledge-articles")]
    public class KnowledgeArticleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public KnowledgeArticleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllKnowledgeArticles(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(
                new GetAllKnowledgeArticlesQuery { IncludeAllStatuses = User.IsInRole("Admin") },
                cancellationToken);
            return Ok(new ApiResponse<List<KnowledgeArticleResponse>>(response));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetKnowledgeArticleById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(
                new GetKnowledgeArticleByIdQuery(id) { IncludeAllStatuses = User.IsInRole("Admin") },
                cancellationToken);
            return Ok(new ApiResponse<KnowledgeArticleResponse>(response));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateKnowledgeArticle([FromBody] CreateKnowledgeArticleCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<Guid>(response, "Article created successfully"));
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateKnowledgeArticles(
            [FromBody] List<CreateKnowledgeArticleCommand> articles,
            CancellationToken cancellationToken)
        {
            var command = new BulkCreateKnowledgeArticlesCommand { Articles = articles };
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<List<Guid>>(response, "Articles created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateKnowledgeArticle(Guid id, [FromBody] UpdateKnowledgeArticleCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id) return BadRequest("Id mismatch");
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Article updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteKnowledgeArticle(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteKnowledgeArticleCommand(id), cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Article deleted successfully"));
        }

        [HttpPost("{id}/reindex")]
        [HttpPost("/api/v1/admin/knowledge-articles/{id:guid}/reindex")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reindex(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new ReindexArticleCommand(id), cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Article reindexed successfully"));
        }
    }
}
