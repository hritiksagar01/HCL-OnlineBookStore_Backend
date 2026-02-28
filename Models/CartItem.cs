namespace Online_BookStore__System.Models;

public class CartItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public int Quantity { get; set; }

    public string? BookTitle { get; set; }
    public string? BookAuthor { get; set; }
    public decimal BookPrice { get; set; }
    public string? BookCoverImageUrl { get; set; }
}

