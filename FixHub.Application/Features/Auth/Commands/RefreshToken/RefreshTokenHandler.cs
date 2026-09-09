

using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.IRepositories;
using MediatR;

namespace FixHub.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
    {
       
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
    
        public RefreshTokenHandler( IJwtService jwtService
               , IUnitOfWork unitOfWork
            )
        {
            _jwtService= jwtService;       
            _unitOfWork= unitOfWork;
          
        }
        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var user = refreshToken.User;
            if (user == null )
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            //revoke the old refresh token
            refreshToken.RevokedAt = DateTime.UtcNow;

            // Generate new access token and refresh token
            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken(user);

            // Save the new refresh token to the database
            await _unitOfWork.RefreshTokenRepository.AddAsync(newRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }
    }
}
