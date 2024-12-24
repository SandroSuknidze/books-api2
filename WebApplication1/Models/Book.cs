namespace WebApplication1.Models
{
    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public Decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
