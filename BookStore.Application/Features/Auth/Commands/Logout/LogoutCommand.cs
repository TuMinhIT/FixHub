using BookStore.Application.Features.Auth.Commands.Logout;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Application.Features.Auth.Commands.logout
{
    public record LogoutCommand(string RefreshToken) : IRequest<bool>;
}
