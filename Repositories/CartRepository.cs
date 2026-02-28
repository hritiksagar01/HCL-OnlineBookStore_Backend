using Npgsql;
using Online_BookStore__System.Database;
using Online_BookStore__System.Models;

namespace Online_BookStore__System.Repositories;

public class CartRepository : ICartRepository
{
    private readonly DbHelper _dbHelper;

    public CartRepository(DbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<IEnumerable<CartItem>> GetByUserIdAsync(int userId)
    {
        var items = new List<CartItem>();
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            @"SELECT ci.id, ci.user_id, ci.book_id, ci.quantity,
                     b.title, b.author, b.price, b.cover_image_url
              FROM cart_items ci
              JOIN books b ON ci.book_id = b.id
              WHERE ci.user_id = @userId
              ORDER BY ci.id", conn);
        cmd.Parameters.AddWithValue("@userId", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new CartItem
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                BookId = reader.GetInt32(2),
                Quantity = reader.GetInt32(3),
                BookTitle = reader.IsDBNull(4) ? null : reader.GetString(4),
                BookAuthor = reader.IsDBNull(5) ? null : reader.GetString(5),
                BookPrice = reader.GetDecimal(6),
                BookCoverImageUrl = reader.IsDBNull(7) ? null : reader.GetString(7)
            });
        }
        return items;
    }

    public async Task<CartItem> AddOrUpdateAsync(int userId, int bookId, int quantity)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            @"INSERT INTO cart_items (user_id, book_id, quantity)
              VALUES (@userId, @bookId, @quantity)
              ON CONFLICT (user_id, book_id)
              DO UPDATE SET quantity = cart_items.quantity + @quantity
              RETURNING id, user_id, book_id, quantity", conn);

        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@bookId", bookId);
        cmd.Parameters.AddWithValue("@quantity", quantity);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new CartItem
        {
            Id = reader.GetInt32(0),
            UserId = reader.GetInt32(1),
            BookId = reader.GetInt32(2),
            Quantity = reader.GetInt32(3)
        };
    }

    public async Task<bool> UpdateQuantityAsync(int id, int userId, int quantity)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "UPDATE cart_items SET quantity = @quantity WHERE id = @id AND user_id = @userId", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@quantity", quantity);

        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> RemoveAsync(int id, int userId)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "DELETE FROM cart_items WHERE id = @id AND user_id = @userId", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@userId", userId);

        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task ClearCartAsync(int userId)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand("DELETE FROM cart_items WHERE user_id = @userId", conn);
        cmd.Parameters.AddWithValue("@userId", userId);
        await cmd.ExecuteNonQueryAsync();
    }
}

