using FixHub.Application.Common.Models;
using FixHub.Application.Features.Categories.Commands.CreateCategory;
using FixHub.Application.Features.Categories.Commands.DeleteCategory;
using FixHub.Application.Features.Categories.Commands.UpdateCategory;
using FixHub.Application.Features.Categories.DTOs;
using FixHub.Application.Features.Categories.Queries.GetAllCategories;
using FixHub.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllCategoriesQuery(), cancellationToken);
            return Ok(new ApiResponse<List<CategoryResponse>>(response));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);
            return Ok(new ApiResponse<CategoryResponse>(response));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<Guid>(response, "Category created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id) return BadRequest("Id mismatch");
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Category updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Category deleted successfully"));
        }
    }
}
