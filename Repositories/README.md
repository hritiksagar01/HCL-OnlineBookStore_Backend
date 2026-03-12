# Repositories

This directory contains the data access layer (DAL) for the Online BookStore application, implementing the Repository pattern.

## Overview

Repositories provide an abstraction over data access, encapsulating all database operations. Each repository is responsible for CRUD operations on a specific entity.

**Pattern**: Repository Pattern
**Data Access**: ADO.NET with PostgreSQL
**Helper**: `DbHelper` for database connection management

## Architecture

### Repository Pattern Benefits
1. **Separation of Concerns**: Data access logic separated from business logic
2. **Testability**: Repositories can be mocked for unit testing
3. **Maintainability**: Database changes isolated to repository layer
4. **Flexibility**: Easy to switch data sources or ORMs

### Structure
Each repository has:
- **Interface**: Defines the contract (IRepository)
- **Implementation**: Contains actual data access code (Repository)

## Repositories

### 1. IUserRepository / UserRepository

Manages user account data.

**Interface Methods**:

#### `GetByUsernameAsync(string username)`
Retrieves a user by username.
- **Returns**: `Task<User?>` - User or null if not found
- **Use Case**: Login, check username availability

#### `GetByEmailAsync(string email)`
Retrieves a user by email address.
- **Returns**: `Task<User?>` - User or null if not found
- **Use Case**: Registration validation, check email availability

#### `CreateAsync(User user)`
Creates a new user account.
- **Returns**: `Task<User>` - Created user with ID
- **Database**: Inserts into `users` table

#### `GetByIdAsync(int id)`
Retrieves a user by ID.
- **Returns**: `Task<User?>` - User or null if not found
- **Use Case**: Get user details

**Example Usage**:
```csharp
var user = await _userRepository.GetByUsernameAsync("johndoe");
if (user != null && BCrypt.Verify(password, user.PasswordHash))
{
    // Login successful
}
```

---

### 2. IBookRepository / BookRepository

Manages book catalog data.

**Interface Methods**:

#### `GetAllAsync(string? category = null, string? search = null)`
Retrieves all books with optional filtering.
- **Parameters**:
  - `category` (optional): Filter by category
  - `search` (optional): Search in title or author
- **Returns**: `Task<IEnumerable<Book>>` - List of books
- **Use Case**: Book listing, search, category filtering

#### `GetByIdAsync(int id)`
Retrieves a specific book by ID.
- **Returns**: `Task<Book?>` - Book or null if not found
- **Use Case**: Book details page

#### `CreateAsync(Book book)`
Adds a new book to the catalog.
- **Returns**: `Task<Book>` - Created book with ID
- **Authorization**: Admin only
- **Database**: Inserts into `books` table

#### `UpdateAsync(int id, Book book)`
Updates an existing book.
- **Returns**: `Task<Book?>` - Updated book or null if not found
- **Authorization**: Admin only
- **Database**: Updates `books` table

#### `DeleteAsync(int id)`
Deletes a book from the catalog.
- **Returns**: `Task<bool>` - True if deleted, false if not found
- **Authorization**: Admin only
- **Database**: Deletes from `books` table (CASCADE deletes related cart items and reviews)

#### `GetCategoriesAsync()`
Gets all unique book categories.
- **Returns**: `Task<IEnumerable<string>>` - List of categories
- **Use Case**: Category filter dropdown

**Example Usage**:
```csharp
// Get all technology books
var techBooks = await _bookRepository.GetAllAsync(category: "Technology");

// Search for books
var searchResults = await _bookRepository.GetAllAsync(search: "clean code");
```

---

### 3. ICartRepository / CartRepository

Manages shopping cart operations.

**Interface Methods**:

#### `GetByUserIdAsync(int userId)`
Gets all cart items for a user.
- **Returns**: `Task<IEnumerable<CartItem>>` - User's cart items
- **Use Case**: Display cart, calculate total

#### `AddOrUpdateAsync(int userId, int bookId, int quantity)`
Adds a book to cart or updates quantity if already exists.
- **Returns**: `Task<CartItem>` - Created or updated cart item
- **Logic**: Uses UPSERT (INSERT ... ON CONFLICT UPDATE)
- **Use Case**: Add to cart button

#### `UpdateQuantityAsync(int id, int userId, int quantity)`
Updates the quantity of a cart item.
- **Returns**: `Task<bool>` - True if updated, false if not found
- **Validation**: Ensures user owns the cart item
- **Use Case**: Change quantity in cart

#### `RemoveAsync(int id, int userId)`
Removes an item from the cart.
- **Returns**: `Task<bool>` - True if removed, false if not found
- **Validation**: Ensures user owns the cart item
- **Use Case**: Remove from cart button

#### `ClearCartAsync(int userId)`
Removes all items from a user's cart.
- **Returns**: `Task` - void
- **Use Case**: After checkout completion

**Example Usage**:
```csharp
// Add book to cart
await _cartRepository.AddOrUpdateAsync(userId: 1, bookId: 5, quantity: 2);

// Get cart items
var cartItems = await _cartRepository.GetByUserIdAsync(userId: 1);
var total = cartItems.Sum(item => item.Quantity * item.Price);
```

---

### 4. IReviewRepository / ReviewRepository

Manages book reviews and ratings.

**Interface Methods**:

#### `GetByBookIdAsync(int bookId)`
Gets all reviews for a specific book.
- **Returns**: `Task<IEnumerable<Review>>` - Book's reviews
- **Use Case**: Display reviews on book detail page

#### `GetAllAsync()`
Gets all reviews in the system.
- **Returns**: `Task<IEnumerable<Review>>` - All reviews
- **Use Case**: Admin review management

#### `CreateAsync(Review review)`
Creates a new review.
- **Returns**: `Task<Review>` - Created review with ID
- **Constraint**: User can only review a book once (enforced by DB)
- **Validation**: Rating must be 1-5
- **Use Case**: Submit review

#### `DeleteAsync(int id)`
Deletes a review.
- **Returns**: `Task<bool>` - True if deleted, false if not found
- **Use Case**: User deletes own review, admin moderation

#### `GetByUserAndBookAsync(int userId, int bookId)`
Gets a specific user's review for a book.
- **Returns**: `Task<Review?>` - Review or null if not found
- **Use Case**: Check if user already reviewed, edit review

**Example Usage**:
```csharp
// Check if user already reviewed
var existingReview = await _reviewRepository.GetByUserAndBookAsync(userId: 1, bookId: 5);
if (existingReview != null)
{
    return BadRequest("You have already reviewed this book");
}

// Create review
await _reviewRepository.CreateAsync(new Review
{
    UserId = 1,
    BookId = 5,
    Rating = 5,
    Comment = "Excellent book!"
});
```

---

### 5. IOrderRepository / OrderRepository

Manages customer orders.

**Interface Methods**:

#### `CreateAsync(Order order)`
Creates a new order.
- **Returns**: `Task<Order>` - Created order with ID
- **Note**: Also creates associated OrderItems
- **Transaction**: Should be wrapped in transaction
- **Use Case**: Checkout process

#### `GetByUserIdAsync(int userId)`
Gets all orders for a user.
- **Returns**: `Task<IEnumerable<Order>>` - User's order history
- **Use Case**: Order history page

#### `GetByIdAsync(int id)`
Gets a specific order by ID.
- **Returns**: `Task<Order?>` - Order details or null
- **Use Case**: Order detail page, order tracking

**Example Usage**:
```csharp
// Create order from cart
var order = new Order
{
    UserId = userId,
    TotalAmount = total,
    Status = "Pending"
};
var createdOrder = await _orderRepository.CreateAsync(order);

// Add order items
foreach (var cartItem in cartItems)
{
    await _orderRepository.CreateOrderItemAsync(new OrderItem
    {
        OrderId = createdOrder.Id,
        BookId = cartItem.BookId,
        Quantity = cartItem.Quantity,
        Price = cartItem.Price
    });
}
```

## Database Connection

All repositories use `DbHelper` to manage database connections:

```csharp
public class DbHelper
{
    private readonly IConfiguration _configuration;

    public DbHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(
            _configuration.GetConnectionString("DefaultConnection")
        );
    }
}
```

## Implementation Pattern

Standard repository method structure:

```csharp
public async Task<Book?> GetByIdAsync(int id)
{
    using var connection = _dbHelper.GetConnection();
    await connection.OpenAsync();

    const string query = "SELECT * FROM books WHERE id = @Id";

    using var command = new NpgsqlCommand(query, connection);
    command.Parameters.AddWithValue("@Id", id);

    using var reader = await command.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
        return MapToBook(reader);
    }

    return null;
}
```

## Best Practices

### 1. Async/Await
All repository methods are asynchronous for better scalability.

### 2. Using Statements
Proper disposal of database resources (connections, commands, readers).

### 3. Parameterized Queries
All queries use parameters to prevent SQL injection.

### 4. Error Handling
- Let exceptions bubble up to be handled by controllers
- Use transactions for multi-statement operations

### 5. Mapping
Separate mapping logic from query logic:
```csharp
private Book MapToBook(NpgsqlDataReader reader)
{
    return new Book
    {
        Id = reader.GetInt32(reader.GetOrdinal("id")),
        Title = reader.GetString(reader.GetOrdinal("title")),
        // ... other fields
    };
}
```

### 6. Null Safety
- Return `null` for not found items (nullable return types)
- Handle DBNull values properly

## Security Considerations

1. **SQL Injection Prevention**: Always use parameterized queries
2. **Access Control**: Verify user ownership in cart/review operations
3. **Data Validation**: Validate input before database operations
4. **Connection Security**: Use SSL for database connections in production
5. **Credentials**: Store connection strings in environment variables

## Transaction Management

For operations requiring multiple queries:

```csharp
public async Task<Order> CheckoutAsync(int userId)
{
    using var connection = _dbHelper.GetConnection();
    await connection.OpenAsync();

    using var transaction = await connection.BeginTransactionAsync();
    try
    {
        // Create order
        // Create order items
        // Clear cart

        await transaction.CommitAsync();
        return order;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

## Testing Repositories

Repositories can be tested with:
1. **Integration Tests**: Test against real database
2. **In-Memory Database**: Test with test database
3. **Mocking**: Mock repositories using interfaces

## Adding New Repositories

To add a new repository:

1. Create interface in this directory
2. Create implementation class
3. Register in `Program.cs`:
   ```csharp
   builder.Services.AddScoped<INewRepository, NewRepository>();
   ```
4. Add database table in `Database/init.sql`
5. Update this README

## Future Enhancements

Potential improvements:
1. **Dapper**: Use micro-ORM for easier mapping
2. **Entity Framework Core**: Full ORM support
3. **Caching**: Add caching layer
4. **Pagination**: Implement pagination for large result sets
5. **Sorting**: Add sorting capabilities
6. **Bulk Operations**: Optimize bulk inserts/updates
