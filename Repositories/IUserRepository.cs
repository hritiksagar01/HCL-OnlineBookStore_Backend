using Online_BookStore__System.Models;

namespace Online_BookStore__System.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(int id);
}

