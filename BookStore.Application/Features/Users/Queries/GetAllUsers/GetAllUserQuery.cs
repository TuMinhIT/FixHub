using MediatR;
using System;

namespace BookStore.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUserQuery: IRequest<List<UserResponse>>
    {
    }
}
