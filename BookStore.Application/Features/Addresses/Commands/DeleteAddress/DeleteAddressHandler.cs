using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.Addresses.Commands.DeleteAddress
{
    public class DeleteAddressHandler : IRequestHandler<DeleteAddressCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteAddressHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            var address = await _unitOfWork.AddressRepository.GetAll()
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (address == null)
            {
                throw new NotFoundException(nameof(Address), request.Id);
            }

            if (address.UserId != _currentUserService.UserId)
            {
                throw new UnauthorizedAccessException("You do not own this address.");
            }

            await _unitOfWork.AddressRepository.DeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
