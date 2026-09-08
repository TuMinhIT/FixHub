using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixHub.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        Guid? UserIdOrNull { get; }
        bool IsAuthenticated { get; }
        string Email { get; }

        string Role { get; }
    }
}
