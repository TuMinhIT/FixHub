using FixHub.Application.Common.Models;
using FixHub.Application.Features.Products.Commands.CreateProduct;
using FixHub.Application.Features.Products.Commands.DeleteProduct;
using FixHub.Application.Features.Products.Commands.UpdateProduct;
using FixHub.Application.Features.Products.Commands.AddProductImage;
using FixHub.Application.Features.Products.Commands.DeleteProductImage;
using FixHub.Application.Features.Products.DTOs;
using FixHub.Application.Features.Products.Queries.GetAllProducts;
using FixHub.Application.Features.Products.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] GetAllProductsQuery query, CancellationToken cancellationToken)
        {
            query.IncludeInactive = query.IncludeInactive && User.IsInRole("Admin");
            var response = await _mediator.Send(query, cancellationToken);
            return Ok(new ApiResponse<Pagination<ProductResponse>>(response));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery(id) { IncludeInactive = User.IsInRole("Admin") };
            var response = await _mediator.Send(query, cancellationToken);
            return Ok(new ApiResponse<ProductResponse>(response));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<Guid>(response, "Product created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id) return BadRequest("Id mismatch");
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Product updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Product deleted successfully"));
        }

        [HttpPost("{productId:guid}/images")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddImage(Guid productId, [FromBody] AddProductImageRequest request, CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(
                new AddProductImageCommand(productId, request.ImageUrl, request.PublicId, request.IsPrimary),
                cancellationToken);
            return Ok(new ApiResponse<Guid>(id, "Product image added successfully"));
        }

        [HttpDelete("{productId:guid}/images/{imageId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImage(Guid productId, Guid imageId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteProductImageCommand(productId, imageId), cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Product image deleted successfully"));
        }
    }

    public sealed class AddProductImageRequest
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string? PublicId { get; set; }
        public bool IsPrimary { get; set; }
    }
}
