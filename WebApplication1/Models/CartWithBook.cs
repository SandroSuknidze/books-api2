namespace WebApplication1.Models
{
    public class CartWithBook
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CartQuantity { get; set; }

    }
}
