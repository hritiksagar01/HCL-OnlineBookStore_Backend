using Online_BookStore__System.Models;

namespace Online_BookStore__System.Repositories;

public interface ICartRepository
{
    Task<IEnumerable<CartItem>> GetByUserIdAsync(int userId);
    Task<CartItem> AddOrUpdateAsync(int userId, int bookId, int quantity);
    Task<bool> UpdateQuantityAsync(int id, int userId, int quantity);
    Task<bool> RemoveAsync(int id, int userId);
    Task ClearCartAsync(int userId);
}

