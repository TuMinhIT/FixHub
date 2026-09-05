using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Addresses.DTOs;
using MediatR;

namespace FixHub.Application.Features.Addresses.Queries.GetMyAddresses
{
    public class GetMyAddressesHandler : IRequestHandler<GetMyAddressesQuery, List<AddressResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetMyAddressesHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<List<AddressResponse>> Handle(GetMyAddressesQuery request, CancellationToken cancellationToken)
        {
            var addresses = await _unitOfWork.AddressRepository.GetByUserIdAsync(_currentUserService.UserId);

            return addresses.Select(a => new AddressResponse
            {
                Id = a.Id,
                UserId = a.UserId,
                Street = a.Street,
                City = a.City,
                State = a.State,
                PostalCode = a.PostalCode,
                Country = a.Country
            }).ToList();
        }
    }
}
