# Controllers

This directory contains all the API controllers that handle HTTP requests and responses for the Online BookStore Backend.

## Overview

Controllers are responsible for:
- Receiving HTTP requests
- Validating input data
- Calling appropriate service/repository methods
- Returning HTTP responses with appropriate status codes

All controllers follow RESTful conventions and return JSON responses.

## Controllers

### 1. AuthController.cs

Handles user authentication and registration.

**Route**: `/api/auth`

**Endpoints**:
- `POST /api/auth/register` - Register a new user
  - **Request Body**: `RegisterDto` (username, email, password, confirmPassword)
  - **Response**: `AuthResponseDto` with JWT token
  - **Status Codes**: 200 (Success), 400 (Validation Error/User Exists)

- `POST /api/auth/login` - Authenticate user and get JWT token
  - **Request Body**: `LoginDto` (username, password)
  - **Response**: `AuthResponseDto` with JWT token and user info
  - **Status Codes**: 200 (Success), 401 (Invalid Credentials)

**Dependencies**: `AuthService`

### 2. BooksController.cs

Manages book catalog and search functionality.

**Route**: `/api/books`

**Endpoints**:
- `GET /api/books` - Get all books
  - **Response**: List of `Book` objects
  - **Status Codes**: 200 (Success)

- `GET /api/books/{id}` - Get book by ID
  - **Parameters**: `id` (int) - Book ID
  - **Response**: `Book` object
  - **Status Codes**: 200 (Success), 404 (Not Found)

- `GET /api/books/search` - Search books by title, author, or category
  - **Query Parameters**: `query` (string) - Search term
  - **Response**: List of matching `Book` objects
  - **Status Codes**: 200 (Success)

**Dependencies**: `IBookRepository`

**Authentication**: Not required (public endpoints)

### 3. CartController.cs

Manages user shopping cart operations.

**Route**: `/api/cart`

**Endpoints**:
- `GET /api/cart` - Get current user's cart items
  - **Response**: List of `CartItemDto` objects
  - **Status Codes**: 200 (Success), 401 (Unauthorized)

- `POST /api/cart` - Add item to cart
  - **Request Body**: `CartItemDto` (bookId, quantity)
  - **Response**: Success message
  - **Status Codes**: 200 (Success), 400 (Invalid Data), 401 (Unauthorized)

- `PUT /api/cart/{bookId}` - Update cart item quantity
  - **Parameters**: `bookId` (int) - Book ID
  - **Request Body**: New quantity
  - **Response**: Success message
  - **Status Codes**: 200 (Success), 404 (Not Found), 401 (Unauthorized)

- `DELETE /api/cart/{bookId}` - Remove item from cart
  - **Parameters**: `bookId` (int) - Book ID
  - **Response**: Success message
  - **Status Codes**: 200 (Success), 404 (Not Found), 401 (Unauthorized)

- `POST /api/cart/checkout` - Create order from cart items
  - **Response**: Order ID
  - **Status Codes**: 200 (Success), 400 (Empty Cart), 401 (Unauthorized)

**Dependencies**: `ICartRepository`, `IOrderRepository`, `IBookRepository`

**Authentication**: Required (JWT Bearer token)

### 4. ReviewsController.cs

Manages book reviews and ratings.

**Route**: `/api/reviews`

**Endpoints**:
- `GET /api/reviews/book/{bookId}` - Get all reviews for a book
  - **Parameters**: `bookId` (int) - Book ID
  - **Response**: List of `ReviewDto` objects
  - **Status Codes**: 200 (Success)

- `POST /api/reviews` - Add a review for a book
  - **Request Body**: `ReviewDto` (bookId, rating, comment)
  - **Response**: Created review
  - **Status Codes**: 201 (Created), 400 (Invalid Data), 401 (Unauthorized)

- `PUT /api/reviews/{id}` - Update own review
  - **Parameters**: `id` (int) - Review ID
  - **Request Body**: `ReviewDto` (rating, comment)
  - **Response**: Updated review
  - **Status Codes**: 200 (Success), 403 (Forbidden), 404 (Not Found), 401 (Unauthorized)

- `DELETE /api/reviews/{id}` - Delete own review
  - **Parameters**: `id` (int) - Review ID
  - **Response**: Success message
  - **Status Codes**: 200 (Success), 403 (Forbidden), 404 (Not Found), 401 (Unauthorized)

**Dependencies**: `IReviewRepository`

**Authentication**: Required for POST/PUT/DELETE (JWT Bearer token)

### 5. AdminController.cs

Administrative functions for managing books and viewing orders.

**Route**: `/api/admin`

**Endpoints**:
- `POST /api/admin/books` - Add a new book
  - **Request Body**: `BookDto` (title, author, price, description, etc.)
  - **Response**: Created book with ID
  - **Status Codes**: 201 (Created), 400 (Invalid Data), 401 (Unauthorized), 403 (Forbidden)

- `PUT /api/admin/books/{id}` - Update book details
  - **Parameters**: `id` (int) - Book ID
  - **Request Body**: `BookDto`
  - **Response**: Success message
  - **Status Codes**: 200 (Success), 404 (Not Found), 401 (Unauthorized), 403 (Forbidden)

- `DELETE /api/admin/books/{id}` - Delete a book
  - **Parameters**: `id` (int) - Book ID
  - **Response**: Success message
  - **Status Codes**: 200 (Success), 404 (Not Found), 401 (Unauthorized), 403 (Forbidden)

- `GET /api/admin/orders` - Get all orders
  - **Response**: List of all orders with details
  - **Status Codes**: 200 (Success), 401 (Unauthorized), 403 (Forbidden)

**Dependencies**: `IBookRepository`, `IOrderRepository`

**Authentication**: Required (JWT Bearer token)
**Authorization**: Admin role only

## Common HTTP Status Codes

- **200 OK**: Request successful
- **201 Created**: Resource created successfully
- **400 Bad Request**: Invalid input data
- **401 Unauthorized**: Authentication required or failed
- **403 Forbidden**: User doesn't have permission
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server error

## Authentication & Authorization

Most endpoints require JWT Bearer token authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

Admin-only endpoints require both authentication and the "Admin" role.

## Error Handling

All controllers return consistent error responses:

```json
{
  "message": "Error description",
  "errors": {
    "field": ["validation error message"]
  }
}
```

## Best Practices

1. **Input Validation**: All DTOs use data annotations for validation
2. **Async/Await**: All controller actions are asynchronous
3. **Dependency Injection**: Controllers receive dependencies via constructor injection
4. **RESTful Design**: Endpoints follow REST conventions
5. **Status Codes**: Appropriate HTTP status codes for all scenarios
6. **Authorization**: Protected endpoints check user authentication and role

## Adding New Controllers

When adding a new controller:

1. Inherit from `ControllerBase`
2. Add `[ApiController]` attribute
3. Define route with `[Route("api/[controller]")]`
4. Use dependency injection for services/repositories
5. Follow async/await pattern
6. Return appropriate HTTP status codes
7. Add XML documentation comments

Example:

```csharp
[ApiController]
[Route("api/[controller]")]
public class NewController : ControllerBase
{
    private readonly IService _service;

    public NewController(IService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }
}
```
