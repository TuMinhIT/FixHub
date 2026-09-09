
using AutoMapper;
using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using MediatR;

namespace FixHub.Application.Features.Auth.Commands.Register
{
    public class RegisterHandler :
        IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        public RegisterHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IMapper mapper   )
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }
        public async Task<RegisterResponse> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
        {

            if (await _unitOfWork.UserRepository.ExistsByEmailAsync(request.Email))
                throw new BadRequestException("Email is already in use.");

            var user = new User
            {
                Email = request.Email,
                Name = request.Name,    
                Password =  _passwordHasher.Hash(request.Password),

            };

            await  _unitOfWork.UserRepository.AddAsync(user);

            await _unitOfWork.SaveChangesAsync();
            
            return _mapper.Map<RegisterResponse>(user);
        }
    }
}
