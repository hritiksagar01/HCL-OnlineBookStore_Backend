using System.ComponentModel.DataAnnotations;

namespace Online_BookStore__System.DTOs;

public class BookDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Author { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 99999.99)]
    public decimal Price { get; set; }

    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Category { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}

