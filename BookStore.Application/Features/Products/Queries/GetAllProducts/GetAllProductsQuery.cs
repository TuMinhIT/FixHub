using FixHub.Application.Features.Products.DTOs;
using MediatR;

namespace FixHub.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQuery : IRequest<List<ProductResponse>>
    {
    }
}
