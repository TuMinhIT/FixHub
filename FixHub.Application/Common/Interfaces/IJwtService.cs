using FixHub.Domain.Entities;
using System;

namespace FixHub.Application.Common.Interfaces
{
    public interface IJwtService
    { 
        public string GenerateAccessToken(User user);

        public RefreshToken GenerateRefreshToken(User user);
       
    }
}
