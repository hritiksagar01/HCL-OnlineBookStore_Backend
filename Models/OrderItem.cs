namespace Online_BookStore__System.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int BookId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public string? BookTitle { get; set; }
}

