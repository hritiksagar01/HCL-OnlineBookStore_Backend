using Online_BookStore__System.Models;

namespace Online_BookStore__System.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
    Task<Order?> GetByIdAsync(int id);
}

