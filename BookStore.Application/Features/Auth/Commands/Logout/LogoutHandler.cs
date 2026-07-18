using BookStore.Application.Common.Interfaces;
using BookStore.Application.Features.Auth.Commands.Logout;

using MediatR;


namespace BookStore.Application.Features.Auth.Commands.logout
{
    public class LogoutHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public LogoutHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
           
            var refreshToken = await  _unitOfWork.RefreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (refreshToken == null)
            {
                return false;
            }
            refreshToken.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

     
    }
}
