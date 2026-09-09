using AutoMapper;
using FixHub.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersHandler
    : IRequestHandler<GetAllUserQuery, List<UserResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllUsersHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<UserResponse>> Handle(
            GetAllUserQuery request,
            CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.UserRepository
                .GetAll()
                .ToListAsync(cancellationToken);
            return _mapper.Map<List<UserResponse>>(users);
        }
    }
}
