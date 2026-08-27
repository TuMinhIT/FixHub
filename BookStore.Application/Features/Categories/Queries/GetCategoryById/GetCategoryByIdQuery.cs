using FixHub.Application.Features.Categories.DTOs;
using MediatR;

namespace FixHub.Application.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQuery : IRequest<CategoryResponse>
    {
        public Guid Id { get; set; }
        public GetCategoryByIdQuery(Guid id) => Id = id;
    }
}
