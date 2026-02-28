using System.ComponentModel.DataAnnotations;

namespace Online_BookStore__System.DTOs;

public class CartItemDto
{
    [Required]
    public int BookId { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; } = 1;
}

