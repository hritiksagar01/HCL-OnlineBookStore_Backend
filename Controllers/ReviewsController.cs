using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_BookStore__System.DTOs;
using Online_BookStore__System.Models;
using Online_BookStore__System.Repositories;

namespace Online_BookStore__System.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewsController(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    [HttpGet("book/{bookId}")]
    public async Task<IActionResult> GetByBookId(int bookId)
    {
        var reviews = await _reviewRepository.GetByBookIdAsync(bookId);
        return Ok(reviews);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ReviewDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Check if user already reviewed this book
        var existing = await _reviewRepository.GetByUserAndBookAsync(userId, dto.BookId);
        if (existing != null)
            return BadRequest(new { message = "You have already reviewed this book." });

        var review = new Review
        {
            UserId = userId,
            BookId = dto.BookId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };

        var created = await _reviewRepository.CreateAsync(review);
        return Ok(created);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _reviewRepository.DeleteAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Review deleted." });
    }
}

