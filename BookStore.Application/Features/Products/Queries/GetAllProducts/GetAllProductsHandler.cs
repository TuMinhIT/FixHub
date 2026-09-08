using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FixHub.Application.Common.Models;

namespace FixHub.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, Pagination<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllProductsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Pagination<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);
            var query = _unitOfWork.ProductRepository.GetAll()
                .Include(x => x.Category)
                .Include(x => x.Images)
                .Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(request.Q))
            {
                var term = request.Q.Trim().ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(term)
                    || (x.Brand != null && x.Brand.ToLower().Contains(term))
                    || (x.Model != null && x.Model.ToLower().Contains(term))
                    || (x.Description != null && x.Description.ToLower().Contains(term)));
            }

            if (request.CategoryId.HasValue)
                query = query.Where(x => x.CategoryId == request.CategoryId.Value);
            if (!string.IsNullOrWhiteSpace(request.Brand))
                query = query.Where(x => x.Brand == request.Brand.Trim());
            if (request.MinPrice.HasValue)
                query = query.Where(x => x.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue)
                query = query.Where(x => x.Price <= request.MaxPrice.Value);
            if (request.CapacityHp.HasValue)
                query = query.Where(x => x.CapacityHp == request.CapacityHp.Value);
            if (request.Inverter.HasValue)
                query = query.Where(x => x.IsInverter == request.Inverter.Value);

            query = request.Sort?.ToLowerInvariant() switch
            {
                "price_asc" => query.OrderBy(x => x.Price),
                "price_desc" => query.OrderByDescending(x => x.Price),
                "name" => query.OrderBy(x => x.Name),
                _ => query.OrderByDescending(x => x.CreatedAt)
            };

            var totalItems = await query.CountAsync(cancellationToken);
            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return new Pagination<ProductResponse>
            {
                Items = _mapper.Map<List<ProductResponse>>(products),
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
    }
}
