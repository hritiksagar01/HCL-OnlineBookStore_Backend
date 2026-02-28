using Microsoft.AspNetCore.Mvc;
using Online_BookStore__System.Repositories;

namespace Online_BookStore__System.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _bookRepository;

    public BooksController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category, [FromQuery] string? search)
    {
        var books = await _bookRepository.GetAllAsync(category, search);
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null) return NotFound();
        return Ok(book);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _bookRepository.GetCategoriesAsync();
        return Ok(categories);
    }
}

