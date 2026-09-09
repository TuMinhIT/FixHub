using FixHub.Application.Features.Categories.DTOs;
using MediatR;

namespace FixHub.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQuery : IRequest<List<CategoryResponse>>
    {
    }
}
