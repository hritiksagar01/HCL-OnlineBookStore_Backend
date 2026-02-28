using Npgsql;
using Online_BookStore__System.Database;
using Online_BookStore__System.Models;

namespace Online_BookStore__System.Repositories;

public class BookRepository : IBookRepository
{
    private readonly DbHelper _dbHelper;

    public BookRepository(DbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<IEnumerable<Book>> GetAllAsync(string? category = null, string? search = null)
    {
        var books = new List<Book>();
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        var sql = "SELECT id, title, author, price, description, cover_image_url, category, stock, created_at FROM books WHERE 1=1";
        if (!string.IsNullOrEmpty(category))
            sql += " AND LOWER(category) = LOWER(@category)";
        if (!string.IsNullOrEmpty(search))
            sql += " AND (LOWER(title) LIKE LOWER(@search) OR LOWER(author) LIKE LOWER(@search))";
        sql += " ORDER BY created_at DESC";

        await using var cmd = new NpgsqlCommand(sql, conn);
        if (!string.IsNullOrEmpty(category))
            cmd.Parameters.AddWithValue("@category", category);
        if (!string.IsNullOrEmpty(search))
            cmd.Parameters.AddWithValue("@search", $"%{search}%");

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            books.Add(MapBook(reader));
        }
        return books;
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, title, author, price, description, cover_image_url, category, stock, created_at FROM books WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapBook(reader) : null;
    }

    public async Task<Book> CreateAsync(Book book)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            @"INSERT INTO books (title, author, price, description, cover_image_url, category, stock)
              VALUES (@title, @author, @price, @description, @coverImageUrl, @category, @stock)
              RETURNING id, created_at", conn);

        cmd.Parameters.AddWithValue("@title", book.Title);
        cmd.Parameters.AddWithValue("@author", book.Author);
        cmd.Parameters.AddWithValue("@price", book.Price);
        cmd.Parameters.AddWithValue("@description", (object?)book.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@coverImageUrl", (object?)book.CoverImageUrl ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@category", (object?)book.Category ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@stock", book.Stock);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            book.Id = reader.GetInt32(0);
            book.CreatedAt = reader.GetDateTime(1);
        }
        return book;
    }

    public async Task<Book?> UpdateAsync(int id, Book book)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            @"UPDATE books SET title = @title, author = @author, price = @price,
              description = @description, cover_image_url = @coverImageUrl,
              category = @category, stock = @stock
              WHERE id = @id RETURNING id, title, author, price, description, cover_image_url, category, stock, created_at", conn);

        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@title", book.Title);
        cmd.Parameters.AddWithValue("@author", book.Author);
        cmd.Parameters.AddWithValue("@price", book.Price);
        cmd.Parameters.AddWithValue("@description", (object?)book.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@coverImageUrl", (object?)book.CoverImageUrl ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@category", (object?)book.Category ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@stock", book.Stock);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapBook(reader) : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand("DELETE FROM books WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        var categories = new List<string>();
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand("SELECT DISTINCT category FROM books WHERE category IS NOT NULL ORDER BY category", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            categories.Add(reader.GetString(0));
        }
        return categories;
    }

    private static Book MapBook(NpgsqlDataReader reader)
    {
        return new Book
        {
            Id = reader.GetInt32(0),
            Title = reader.GetString(1),
            Author = reader.GetString(2),
            Price = reader.GetDecimal(3),
            Description = reader.IsDBNull(4) ? null : reader.GetString(4),
            CoverImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
            Category = reader.IsDBNull(6) ? null : reader.GetString(6),
            Stock = reader.GetInt32(7),
            CreatedAt = reader.GetDateTime(8)
        };
    }
}

