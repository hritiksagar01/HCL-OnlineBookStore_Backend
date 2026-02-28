using Npgsql;
using Online_BookStore__System.Database;
using Online_BookStore__System.Models;

namespace Online_BookStore__System.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly DbHelper _dbHelper;

    public ReviewRepository(DbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<IEnumerable<Review>> GetByBookIdAsync(int bookId)
    {
        var reviews = new List<Review>();
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            @"SELECT r.id, r.user_id, r.book_id, r.rating, r.comment, r.created_at, u.username, b.title
              FROM reviews r
              JOIN users u ON r.user_id = u.id
              JOIN books b ON r.book_id = b.id
              WHERE r.book_id = @bookId
              ORDER BY r.created_at DESC", conn);
        cmd.Parameters.AddWithValue("@bookId", bookId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            reviews.Add(MapReview(reader));
        }
        return reviews;
    }

    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        var reviews = new List<Review>();
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            @"SELECT r.id, r.user_id, r.book_id, r.rating, r.comment, r.created_at, u.username, b.title
              FROM reviews r
              JOIN users u ON r.user_id = u.id
              JOIN books b ON r.book_id = b.id
              ORDER BY r.created_at DESC", conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            reviews.Add(MapReview(reader));
        }
        return reviews;
    }

    public async Task<Review> CreateAsync(Review review)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            @"INSERT INTO reviews (user_id, book_id, rating, comment)
              VALUES (@userId, @bookId, @rating, @comment)
              RETURNING id, created_at", conn);

        cmd.Parameters.AddWithValue("@userId", review.UserId);
        cmd.Parameters.AddWithValue("@bookId", review.BookId);
        cmd.Parameters.AddWithValue("@rating", review.Rating);
        cmd.Parameters.AddWithValue("@comment", (object?)review.Comment ?? DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            review.Id = reader.GetInt32(0);
            review.CreatedAt = reader.GetDateTime(1);
        }
        return review;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand("DELETE FROM reviews WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<Review?> GetByUserAndBookAsync(int userId, int bookId)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            @"SELECT r.id, r.user_id, r.book_id, r.rating, r.comment, r.created_at, u.username, b.title
              FROM reviews r
              JOIN users u ON r.user_id = u.id
              JOIN books b ON r.book_id = b.id
              WHERE r.user_id = @userId AND r.book_id = @bookId", conn);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@bookId", bookId);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapReview(reader) : null;
    }

    private static Review MapReview(NpgsqlDataReader reader)
    {
        return new Review
        {
            Id = reader.GetInt32(0),
            UserId = reader.GetInt32(1),
            BookId = reader.GetInt32(2),
            Rating = reader.GetInt32(3),
            Comment = reader.IsDBNull(4) ? null : reader.GetString(4),
            CreatedAt = reader.GetDateTime(5),
            Username = reader.GetString(6),
            BookTitle = reader.GetString(7)
        };
    }
}

