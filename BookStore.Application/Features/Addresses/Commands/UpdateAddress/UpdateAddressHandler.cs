using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.Addresses.Commands.UpdateAddress
{
    public class UpdateAddressHandler : IRequestHandler<UpdateAddressCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateAddressHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Street) ||
                string.IsNullOrWhiteSpace(request.City) ||
                string.IsNullOrWhiteSpace(request.State) ||
                string.IsNullOrWhiteSpace(request.PostalCode) ||
                string.IsNullOrWhiteSpace(request.Country))
            {
                throw new BadRequestException("All address fields are required.");
            }

            var address = await _unitOfWork.AddressRepository.GetAll()
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (address == null)
            {
                throw new NotFoundException(nameof(Address), request.Id);
            }

            if (address.UserId != _currentUserService.UserId)
            {
                throw new ForbiddenException("You do not own this address.");
            }

            address.Street = request.Street.Trim();
            address.City = request.City.Trim();
            address.State = request.State.Trim();
            address.PostalCode = request.PostalCode.Trim();
            address.Country = request.Country.Trim();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
