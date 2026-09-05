using MediatR;

namespace FixHub.Application.Features.Addresses.Commands.DeleteAddress
{
    public class DeleteAddressCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteAddressCommand(Guid id)
        {
            Id = id;
        }
    }
}
