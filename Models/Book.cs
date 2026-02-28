namespace Online_BookStore__System.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Category { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
}
