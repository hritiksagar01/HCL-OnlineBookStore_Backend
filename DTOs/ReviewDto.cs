using System.ComponentModel.DataAnnotations;

namespace Online_BookStore__System.DTOs;

public class ReviewDto
{
    [Required]
    public int BookId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Comment { get; set; }
}

