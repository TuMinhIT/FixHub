using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixHub.Domain.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string? ISBN { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int stock { get; set; }
        public string? Author { get; set; }
        public decimal Price { get; set; }     
        public List<string> Images { get; set; } = new List<string>();
        public string? Language { get; set; }
        public int? PublishYear { get; set; }
        public int? PageCount { get; set; }

        public DateTime? Created { get; set; }= DateTime.UtcNow;
        public DateTime? Updated { get; set; }
        // navigation properties

        public BookCategory Category { get; set; } = null!;
    }
}



