using MediatR;

namespace FixHub.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUserQuery: IRequest<List<UserResponse>>
    {
    }
}
