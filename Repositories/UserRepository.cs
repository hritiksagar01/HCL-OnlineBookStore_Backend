using Npgsql;
using Online_BookStore__System.Database;
using Online_BookStore__System.Models;

namespace Online_BookStore__System.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbHelper _dbHelper;

    public UserRepository(DbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, username, email, password_hash, role, created_at FROM users WHERE LOWER(username) = LOWER(@username)", conn);
        cmd.Parameters.AddWithValue("@username", username);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapUser(reader) : null;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, username, email, password_hash, role, created_at FROM users WHERE LOWER(email) = LOWER(@email)", conn);
        cmd.Parameters.AddWithValue("@email", email);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapUser(reader) : null;
    }

    public async Task<User> CreateAsync(User user)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            @"INSERT INTO users (username, email, password_hash, role)
              VALUES (@username, @email, @passwordHash, @role)
              RETURNING id, created_at", conn);

        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@email", user.Email);
        cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
        cmd.Parameters.AddWithValue("@role", user.Role);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            user.Id = reader.GetInt32(0);
            user.CreatedAt = reader.GetDateTime(1);
        }
        return user;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, username, email, password_hash, role, created_at FROM users WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapUser(reader) : null;
    }

    private static User MapUser(NpgsqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetInt32(0),
            Username = reader.GetString(1),
            Email = reader.GetString(2),
            PasswordHash = reader.GetString(3),
            Role = reader.GetString(4),
            CreatedAt = reader.GetDateTime(5)
        };
    }
}

