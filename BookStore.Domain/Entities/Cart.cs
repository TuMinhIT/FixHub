using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Domain.Entities
{
    public class Cart
    {
        public Guid  Id { get; set; }
        public Guid UserId { get; set; }    
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public User User { get; set; } = null!;

    }
}
