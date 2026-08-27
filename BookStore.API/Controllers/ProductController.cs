using FixHub.Application.Common.Models;
using FixHub.Application.Features.Products.Commands.CreateProduct;
using FixHub.Application.Features.Products.Commands.DeleteProduct;
using FixHub.Application.Features.Products.Commands.UpdateProduct;
using FixHub.Application.Features.Products.DTOs;
using FixHub.Application.Features.Products.Queries.GetAllProducts;
using FixHub.Application.Features.Products.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllProductsQuery(), cancellationToken);
            return Ok(new ApiResponse<List<ProductResponse>>(response));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
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
    }
}
