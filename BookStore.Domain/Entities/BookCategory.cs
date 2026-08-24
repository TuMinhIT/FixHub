namespace FixHub.Domain.Entities
{
    public class BookCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<Book> Books { get; set; } = new List<Book>();
       
    }
}
