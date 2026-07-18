using AutoMapper;
using BookStore.Application.Common.Interfaces;
using BookStore.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;


namespace BookStore.Application.Features.Auth.Commands.Login
{
   public class LoginHandler: IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly ILogger _logger;
        public LoginHandler(IUnitOfWork unitOfWork,IPasswordHasher passwordHasher, IMapper mapper,IJwtService jwtService, ILogger<LoginHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;

        }
        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            User user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
                     
            if (user == null || !_passwordHasher.Verify(request.Password, user.Password))
            {
                throw new Exception("Invalid email or password");
            }

            if (!user.IsActive)
            {
                throw new Exception("User is not active");
            }
       
            //generate access token and refresh token
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user.Id);

            await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponse
            {
                RefeshToken = refreshToken.Token,
                AccessToken = accessToken,
                Expiration = DateTime.UtcNow.AddMinutes(30),
                User = _mapper.Map<UserLoginResponse>(user)
            };                
        }
    }
}
