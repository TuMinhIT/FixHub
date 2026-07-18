

namespace BookStore.Application.Features.Auth.Commands.Logout
{
    public class LogoutResponse
    {
        public bool IsSuccess { get; }

        public LogoutResponse(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

       
    }
}
