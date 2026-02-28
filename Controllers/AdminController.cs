using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_BookStore__System.DTOs;
using Online_BookStore__System.Models;
using Online_BookStore__System.Repositories;

namespace Online_BookStore__System.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    private readonly IReviewRepository _reviewRepository;

    public AdminController(IBookRepository bookRepository, IReviewRepository reviewRepository)
    {
        _bookRepository = bookRepository;
        _reviewRepository = reviewRepository;
    }
    
    [HttpPost("books")]
    public async Task<IActionResult> CreateBook([FromBody] BookDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var book = new Book
        {
            Title = dto.Title,
            Author = dto.Author,
            Price = dto.Price,
            Description = dto.Description,
            CoverImageUrl = dto.CoverImageUrl,
            Category = dto.Category,
            Stock = dto.Stock
        };

        var created = await _bookRepository.CreateAsync(book);
        return Ok(created);
    }

    [HttpPut("books/{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] BookDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var book = new Book
        {
            Title = dto.Title,
            Author = dto.Author,
            Price = dto.Price,
            Description = dto.Description,
            CoverImageUrl = dto.CoverImageUrl,
            Category = dto.Category,
            Stock = dto.Stock
        };

        var updated = await _bookRepository.UpdateAsync(id, book);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("books/{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var result = await _bookRepository.DeleteAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Book deleted." });
    }

    // ===== Review Management =====

    [HttpGet("reviews")]
    public async Task<IActionResult> GetAllReviews()
    {
        var reviews = await _reviewRepository.GetAllAsync();
        return Ok(reviews);
    }

    [HttpDelete("reviews/{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var result = await _reviewRepository.DeleteAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Review deleted." });
    }
}

