using FixHub.Application.Features.Products.DTOs;
using MediatR;

namespace FixHub.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<ProductResponse>
    {
        public Guid Id { get; set; }
        public GetProductByIdQuery(Guid id) => Id = id;
    }
}
