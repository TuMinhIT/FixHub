using AutoMapper;
using FixHub.Application.Features.Categories.Commands.CreateCategory;
using FixHub.Application.Features.Categories.Commands.UpdateCategory;
using FixHub.Application.Features.Categories.DTOs;
using FixHub.Domain.Entities;

namespace FixHub.Application.Common.Mappings
{
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            CreateMap<Category, CategoryResponse>();
            CreateMap<CreateCategoryCommand, Category>();
            CreateMap<UpdateCategoryCommand, Category>();
        }
    }
}
