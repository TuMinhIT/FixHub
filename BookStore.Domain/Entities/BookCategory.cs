namespace BookStore.Domain.Entities
{
    public class BookCategory
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; }

    }
}
