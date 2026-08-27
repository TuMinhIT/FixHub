using FixHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixHub.Application.Features.Users.Queries.GetProfile
{
    public class GetProfileResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreateAt { get; set; }
        = DateTime.UtcNow;
        public DateTime? UpdateAt { get; set; }
    }
}
