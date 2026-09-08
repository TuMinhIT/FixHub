using AutoMapper;
using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using MediatR;

namespace FixHub.Application.Features.Users.Commands.UpdateInfo
{
    public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, UpdateProfileResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateProfileHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<UpdateProfileResponse> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BadRequestException("Name is required.");
            }

            var userId = _currentUserService.UserId;
            var user = await _unitOfWork.UserRepository.FindById(userId);

            if (user == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            user.Name = request.Name.Trim();
            user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? user.PhoneNumber : request.PhoneNumber.Trim();
            user.Gender = string.IsNullOrWhiteSpace(request.Gender) ? user.Gender : request.Gender.Trim();

            if (request.Dob.HasValue)
            {
                var dob = request.Dob.Value;

                if (dob.Kind == DateTimeKind.Unspecified)
                {
                    dob = DateTime.SpecifyKind(dob, DateTimeKind.Utc);
                }
                else if (dob.Kind == DateTimeKind.Local)
                {
                    dob = dob.ToUniversalTime();
                }

                user.Dob = dob;
            }

            if (!string.IsNullOrWhiteSpace(request.Avatar))
            {
                user.Avatar = request.Avatar.Trim();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<UpdateProfileResponse>(user);
        }
    }
}
