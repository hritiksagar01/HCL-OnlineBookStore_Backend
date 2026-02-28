using Online_BookStore__System.Models;

namespace Online_BookStore__System.Repositories;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetByBookIdAsync(int bookId);
    Task<IEnumerable<Review>> GetAllAsync();
    Task<Review> CreateAsync(Review review);
    Task<bool> DeleteAsync(int id);
    Task<Review?> GetByUserAndBookAsync(int userId, int bookId);
}

