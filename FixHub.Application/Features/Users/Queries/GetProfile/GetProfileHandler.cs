using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Exceptions;
using FixHub.Domain.Entities;
using MediatR;


namespace FixHub.Application.Features.Users.Queries.GetProfile
{
    public class GetProfileHandler : IRequestHandler<GetProfileQuery, GetProfileResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        public GetProfileHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<GetProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
          var userId = _currentUserService.UserId;
          var user =  await _unitOfWork.UserRepository.FindById(userId);
            if (user == null)
                throw new NotFoundException(nameof(User), userId);
            return _mapper.Map<GetProfileResponse>(user);
        }
    }
}
