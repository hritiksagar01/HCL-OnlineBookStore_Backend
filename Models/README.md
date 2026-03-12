# Models

This directory contains the domain models (entities) that represent the core business objects in the Online BookStore application.

## Overview

Models represent the data structure and business logic entities that map to database tables. Each model corresponds to a table in the PostgreSQL database and contains properties that map to columns.

## Models

### 1. User.cs

Represents a user account in the system.

**Database Table**: `users`

**Properties**:
- `Id` (int): Primary key, auto-generated
- `Username` (string): Unique username for login
- `Email` (string): Unique email address
- `PasswordHash` (string): Hashed password (BCrypt)
- `Role` (string): User role ("User" or "Admin"), defaults to "User"
- `CreatedAt` (DateTime): Account creation timestamp

**Relationships**:
- One-to-Many with `CartItem` (a user can have multiple cart items)
- One-to-Many with `Order` (a user can place multiple orders)
- One-to-Many with `Review` (a user can write multiple reviews)

**Usage**:
```csharp
var user = new User
{
    Username = "johndoe",
    Email = "john@example.com",
    PasswordHash = hashedPassword,
    Role = "User"
};
```

### 2. Book.cs

Represents a book in the catalog.

**Database Table**: `books`

**Properties**:
- `Id` (int): Primary key, auto-generated
- `Title` (string): Book title
- `Author` (string): Book author name
- `Price` (decimal): Book price (10,2 precision)
- `Description` (string?): Optional book description
- `CoverImageUrl` (string?): Optional URL to book cover image
- `Category` (string?): Optional book category/genre
- `Stock` (int): Available quantity in inventory
- `CreatedAt` (DateTime): Record creation timestamp

**Relationships**:
- One-to-Many with `CartItem` (a book can be in multiple carts)
- One-to-Many with `OrderItem` (a book can be in multiple orders)
- One-to-Many with `Review` (a book can have multiple reviews)

**Usage**:
```csharp
var book = new Book
{
    Title = "Clean Code",
    Author = "Robert C. Martin",
    Price = 34.99m,
    Description = "A handbook of agile software craftsmanship",
    Category = "Technology",
    Stock = 20
};
```

### 3. CartItem.cs

Represents an item in a user's shopping cart.

**Database Table**: `cart_items`

**Properties**:
- `Id` (int): Primary key, auto-generated
- `UserId` (int): Foreign key to User
- `BookId` (int): Foreign key to Book
- `Quantity` (int): Number of books, defaults to 1

**Relationships**:
- Many-to-One with `User` (cart item belongs to one user)
- Many-to-One with `Book` (cart item references one book)

**Constraints**:
- Unique constraint on (UserId, BookId) - a user can only have one cart item per book

**Usage**:
```csharp
var cartItem = new CartItem
{
    UserId = 1,
    BookId = 5,
    Quantity = 2
};
```

### 4. Order.cs

Represents a customer order.

**Database Table**: `orders`

**Properties**:
- `Id` (int): Primary key, auto-generated
- `UserId` (int): Foreign key to User
- `TotalAmount` (decimal): Total order amount (10,2 precision)
- `Status` (string): Order status, defaults to "Pending"
- `CreatedAt` (DateTime): Order creation timestamp

**Relationships**:
- Many-to-One with `User` (order belongs to one user)
- One-to-Many with `OrderItem` (order contains multiple items)

**Status Values**:
- "Pending": Order placed but not processed
- "Processing": Order being prepared
- "Shipped": Order dispatched
- "Delivered": Order completed
- "Cancelled": Order cancelled

**Usage**:
```csharp
var order = new Order
{
    UserId = 1,
    TotalAmount = 89.97m,
    Status = "Pending"
};
```

### 5. OrderItem.cs

Represents an individual item within an order.

**Database Table**: `order_items`

**Properties**:
- `Id` (int): Primary key, auto-generated
- `OrderId` (int): Foreign key to Order
- `BookId` (int): Foreign key to Book
- `Quantity` (int): Number of books ordered
- `Price` (decimal): Price per book at time of order (10,2 precision)

**Relationships**:
- Many-to-One with `Order` (item belongs to one order)
- Many-to-One with `Book` (item references one book)

**Note**: Price is stored at order time to maintain historical accuracy even if book price changes later.

**Usage**:
```csharp
var orderItem = new OrderItem
{
    OrderId = 1,
    BookId = 5,
    Quantity = 2,
    Price = 34.99m
};
```

### 6. Review.cs

Represents a user review for a book.

**Database Table**: `reviews`

**Properties**:
- `Id` (int): Primary key, auto-generated
- `UserId` (int): Foreign key to User
- `BookId` (int): Foreign key to Book
- `Rating` (int): Rating from 1 to 5 stars
- `Comment` (string): Review text/comment
- `CreatedAt` (DateTime): Review creation timestamp

**Relationships**:
- Many-to-One with `User` (review written by one user)
- Many-to-One with `Book` (review is for one book)

**Constraints**:
- Unique constraint on (UserId, BookId) - a user can only review a book once
- Rating must be between 1 and 5 (CHECK constraint)

**Usage**:
```csharp
var review = new Review
{
    UserId = 1,
    BookId = 5,
    Rating = 5,
    Comment = "Excellent book! Highly recommended."
};
```

## Entity Relationships

```
User
  ├─→ CartItem (1-to-Many)
  ├─→ Order (1-to-Many)
  └─→ Review (1-to-Many)

Book
  ├─→ CartItem (1-to-Many)
  ├─→ OrderItem (1-to-Many)
  └─→ Review (1-to-Many)

Order
  └─→ OrderItem (1-to-Many)
```

## Design Principles

### 1. Property Initialization
All string properties are initialized to `string.Empty` to avoid null reference warnings:
```csharp
public string Title { get; set; } = string.Empty;
```

Optional properties use nullable reference types:
```csharp
public string? Description { get; set; }
```

### 2. DateTime Handling
All timestamps use `DateTime` type and are set by the database using `DEFAULT NOW()`.

### 3. Decimal Precision
All monetary values use `decimal` type with (10,2) precision in the database.

### 4. Naming Conventions
- PascalCase for all property names (C# convention)
- Database columns use snake_case (PostgreSQL convention)
- Mapping is handled by the repositories

## Database Constraints

### Unique Constraints
- `users.username` - Each username must be unique
- `users.email` - Each email must be unique
- `cart_items(user_id, book_id)` - User can't have duplicate items for same book
- `reviews(user_id, book_id)` - User can only review a book once

### Foreign Key Constraints
All foreign keys have appropriate CASCADE or RESTRICT behaviors:
- `cart_items`: ON DELETE CASCADE (delete cart items when book/user deleted)
- `reviews`: ON DELETE CASCADE (delete reviews when book/user deleted)
- `order_items`: ON DELETE CASCADE (delete order items when order deleted)

### Check Constraints
- `reviews.rating`: Must be between 1 and 5

## Best Practices

1. **Immutability**: Consider making models immutable where appropriate
2. **Validation**: Use data annotations in DTOs rather than models
3. **Business Logic**: Keep models simple; business logic goes in services
4. **Relationships**: Use navigation properties when using an ORM (not currently implemented)
5. **Timestamps**: Let the database handle timestamps rather than application code

## Adding New Models

When adding a new model:

1. Create the class in this directory
2. Define all properties with appropriate types
3. Add summary XML comments
4. Update database schema in `Database/init.sql`
5. Create corresponding repository interface and implementation
6. Create corresponding DTO if needed
7. Update this README

Example:

```csharp
namespace Online_BookStore__System.Models;

/// <summary>
/// Represents a customer wishlist item
/// </summary>
public class WishlistItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public DateTime CreatedAt { get; set; }
}
```
