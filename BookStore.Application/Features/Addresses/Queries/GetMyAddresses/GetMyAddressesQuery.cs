using FixHub.Application.Features.Addresses.DTOs;
using MediatR;

namespace FixHub.Application.Features.Addresses.Queries.GetMyAddresses
{
    public class GetMyAddressesQuery : IRequest<List<AddressResponse>>
    {
    }
}
