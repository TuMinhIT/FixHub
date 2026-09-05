using MediatR;

namespace FixHub.Application.Features.Addresses.Commands.CreateAddress
{
    public class CreateAddressCommand : IRequest<Guid>
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
