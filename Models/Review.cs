namespace Online_BookStore__System.Models;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    public string? Username { get; set; }
    public string? BookTitle { get; set; }
}

