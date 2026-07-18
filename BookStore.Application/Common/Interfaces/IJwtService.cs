using BookStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Application.Common.Interfaces
{
    public interface IJwtService
    { 
        public string GenerateAccessToken(User user);

        public RefreshToken GenerateRefreshToken(User user);
       
    }
}
