using Npgsql;
using Online_BookStore__System.Database;
using Online_BookStore__System.Models;

namespace Online_BookStore__System.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly DbHelper _dbHelper;

    public OrderRepository(DbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<Order> CreateAsync(Order order)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            // Create order
            await using var orderCmd = new NpgsqlCommand(
                @"INSERT INTO orders (user_id, total_amount, status)
                  VALUES (@userId, @totalAmount, @status)
                  RETURNING id, created_at", conn, transaction);

            orderCmd.Parameters.AddWithValue("@userId", order.UserId);
            orderCmd.Parameters.AddWithValue("@totalAmount", order.TotalAmount);
            orderCmd.Parameters.AddWithValue("@status", order.Status);

            await using var reader = await orderCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                order.Id = reader.GetInt32(0);
                order.CreatedAt = reader.GetDateTime(1);
            }
            await reader.CloseAsync();

            // Create order items
            foreach (var item in order.Items)
            {
                await using var itemCmd = new NpgsqlCommand(
                    @"INSERT INTO order_items (order_id, book_id, quantity, price)
                      VALUES (@orderId, @bookId, @quantity, @price)
                      RETURNING id", conn, transaction);

                itemCmd.Parameters.AddWithValue("@orderId", order.Id);
                itemCmd.Parameters.AddWithValue("@bookId", item.BookId);
                itemCmd.Parameters.AddWithValue("@quantity", item.Quantity);
                itemCmd.Parameters.AddWithValue("@price", item.Price);

                item.Id = (int)(await itemCmd.ExecuteScalarAsync())!;
                item.OrderId = order.Id;
            }

            await transaction.CommitAsync();
            return order;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
    {
        var orders = new List<Order>();
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            @"SELECT id, user_id, total_amount, status, created_at
              FROM orders WHERE user_id = @userId ORDER BY created_at DESC", conn);
        cmd.Parameters.AddWithValue("@userId", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            orders.Add(new Order
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                TotalAmount = reader.GetDecimal(2),
                Status = reader.GetString(3),
                CreatedAt = reader.GetDateTime(4)
            });
        }
        await reader.CloseAsync();

        // Load order items for each order
        foreach (var order in orders)
        {
            await using var itemCmd = new NpgsqlCommand(
                @"SELECT oi.id, oi.order_id, oi.book_id, oi.quantity, oi.price, b.title
                  FROM order_items oi
                  JOIN books b ON oi.book_id = b.id
                  WHERE oi.order_id = @orderId", conn);
            itemCmd.Parameters.AddWithValue("@orderId", order.Id);

            await using var itemReader = await itemCmd.ExecuteReaderAsync();
            while (await itemReader.ReadAsync())
            {
                order.Items.Add(new OrderItem
                {
                    Id = itemReader.GetInt32(0),
                    OrderId = itemReader.GetInt32(1),
                    BookId = itemReader.GetInt32(2),
                    Quantity = itemReader.GetInt32(3),
                    Price = itemReader.GetDecimal(4),
                    BookTitle = itemReader.IsDBNull(5) ? null : itemReader.GetString(5)
                });
            }
        }

        return orders;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        await using var conn = _dbHelper.GetConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, user_id, total_amount, status, created_at FROM orders WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        var order = new Order
        {
            Id = reader.GetInt32(0),
            UserId = reader.GetInt32(1),
            TotalAmount = reader.GetDecimal(2),
            Status = reader.GetString(3),
            CreatedAt = reader.GetDateTime(4)
        };
        await reader.CloseAsync();

        await using var itemCmd = new NpgsqlCommand(
            @"SELECT oi.id, oi.order_id, oi.book_id, oi.quantity, oi.price, b.title
              FROM order_items oi
              JOIN books b ON oi.book_id = b.id
              WHERE oi.order_id = @orderId", conn);
        itemCmd.Parameters.AddWithValue("@orderId", order.Id);

        await using var itemReader = await itemCmd.ExecuteReaderAsync();
        while (await itemReader.ReadAsync())
        {
            order.Items.Add(new OrderItem
            {
                Id = itemReader.GetInt32(0),
                OrderId = itemReader.GetInt32(1),
                BookId = itemReader.GetInt32(2),
                Quantity = itemReader.GetInt32(3),
                Price = itemReader.GetDecimal(4),
                BookTitle = itemReader.IsDBNull(5) ? null : itemReader.GetString(5)
            });
        }

        return order;
    }
}

