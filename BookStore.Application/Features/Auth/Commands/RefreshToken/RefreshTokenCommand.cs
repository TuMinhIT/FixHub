

using MediatR;

namespace BookStore.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand: IRequest<RefreshTokenResponse>
    {
        public string RefreshToken { get; set; }
        public RefreshTokenCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}
