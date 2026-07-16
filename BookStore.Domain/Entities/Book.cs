using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Domain.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }     
        public string image { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        //public Category Category { get; set; }

        // navigation properties
        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();


    }
}
