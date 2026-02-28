using Online_BookStore__System.Models;

namespace Online_BookStore__System.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync(string? category = null, string? search = null);
    Task<Book?> GetByIdAsync(int id);
    Task<Book> CreateAsync(Book book);
    Task<Book?> UpdateAsync(int id, Book book);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<string>> GetCategoriesAsync();
}
