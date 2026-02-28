using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_BookStore__System.DTOs;
using Online_BookStore__System.Models;
using Online_BookStore__System.Repositories;

namespace Online_BookStore__System.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IBookRepository _bookRepository;

    public CartController(ICartRepository cartRepository, IOrderRepository orderRepository, IBookRepository bookRepository)
    {
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
        _bookRepository = bookRepository;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();
        var items = await _cartRepository.GetByUserIdAsync(userId);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] CartItemDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetUserId();
        var book = await _bookRepository.GetByIdAsync(dto.BookId);
        if (book == null) return NotFound(new { message = "Book not found." });

        var item = await _cartRepository.AddOrUpdateAsync(userId, dto.BookId, dto.Quantity);
        return Ok(item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuantity(int id, [FromBody] CartItemDto dto)
    {
        var userId = GetUserId();
        var result = await _cartRepository.UpdateQuantityAsync(id, userId, dto.Quantity);
        if (!result) return NotFound();
        return Ok(new { message = "Cart updated." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        var userId = GetUserId();
        var result = await _cartRepository.RemoveAsync(id, userId);
        if (!result) return NotFound();
        return Ok(new { message = "Item removed from cart." });
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var userId = GetUserId();
        var cartItems = (await _cartRepository.GetByUserIdAsync(userId)).ToList();

        if (!cartItems.Any())
            return BadRequest(new { message = "Cart is empty." });

        var order = new Order
        {
            UserId = userId,
            TotalAmount = cartItems.Sum(ci => ci.BookPrice * ci.Quantity),
            Status = "Completed",
            Items = cartItems.Select(ci => new OrderItem
            {
                BookId = ci.BookId,
                Quantity = ci.Quantity,
                Price = ci.BookPrice
            }).ToList()
        };

        var createdOrder = await _orderRepository.CreateAsync(order);
        await _cartRepository.ClearCartAsync(userId);

        return Ok(createdOrder);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders()
    {
        var userId = GetUserId();
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        return Ok(orders);
    }
}

