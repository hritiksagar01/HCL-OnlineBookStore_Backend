# API Documentation

Complete API reference for the HCL Online BookStore Backend.

## Base URL

- **Development**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `http://localhost:5000/swagger`

## Authentication

Most endpoints require JWT Bearer token authentication.

### How to Authenticate

1. Obtain a token via `/api/auth/register` or `/api/auth/login`
2. Include the token in the Authorization header:
   ```
   Authorization: Bearer <your-jwt-token>
   ```

## Response Codes

| Code | Description |
|------|-------------|
| 200  | OK - Request successful |
| 201  | Created - Resource created successfully |
| 400  | Bad Request - Invalid input or validation error |
| 401  | Unauthorized - Authentication required or failed |
| 403  | Forbidden - Insufficient permissions |
| 404  | Not Found - Resource not found |
| 500  | Internal Server Error - Server error |

## Error Response Format

```json
{
  "message": "Error description",
  "errors": {
    "field": ["validation error message"]
  }
}
```

---

## Authentication Endpoints

### Register User

Creates a new user account.

**Endpoint**: `POST /api/auth/register`

**Authentication**: Not required

**Request Body**:
```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Request Fields**:
| Field | Type | Required | Validation |
|-------|------|----------|------------|
| username | string | Yes | Min 3 characters |
| email | string | Yes | Valid email format |
| password | string | Yes | Min 6 characters |

**Success Response** (200):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "johndoe",
  "role": "User"
}
```

**Error Responses**:
- **400 Bad Request**: Username or email already exists
- **400 Bad Request**: Validation errors

**Example**:
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "email": "john@example.com",
    "password": "SecurePass123!"
  }'
```

---

### Login

Authenticates a user and returns a JWT token.

**Endpoint**: `POST /api/auth/login`

**Authentication**: Not required

**Request Body**:
```json
{
  "username": "johndoe",
  "password": "SecurePass123!"
}
```

**Request Fields**:
| Field | Type | Required |
|-------|------|----------|
| username | string | Yes |
| password | string | Yes |

**Success Response** (200):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "johndoe",
  "role": "User"
}
```

**Error Responses**:
- **401 Unauthorized**: Invalid username or password

**Example**:
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "password": "SecurePass123!"
  }'
```

**Default Admin Credentials**:
- Username: `admin`
- Password: `Admin@123`

---

## Book Endpoints

### Get All Books

Retrieves all books in the catalog.

**Endpoint**: `GET /api/books`

**Authentication**: Not required

**Query Parameters**:
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| category | string | No | Filter by category |
| search | string | No | Search in title or author |

**Success Response** (200):
```json
[
  {
    "id": 1,
    "title": "Clean Code",
    "author": "Robert C. Martin",
    "price": 34.99,
    "description": "A handbook of agile software craftsmanship",
    "coverImageUrl": "https://example.com/cover.jpg",
    "category": "Technology",
    "stock": 20,
    "createdAt": "2024-01-15T10:30:00Z"
  }
]
```

**Examples**:
```bash
# Get all books
curl http://localhost:5000/api/books

# Filter by category
curl http://localhost:5000/api/books?category=Technology

# Search
curl http://localhost:5000/api/books?search=clean
```

---

### Get Book by ID

Retrieves a specific book by its ID.

**Endpoint**: `GET /api/books/{id}`

**Authentication**: Not required

**Path Parameters**:
| Parameter | Type | Description |
|-----------|------|-------------|
| id | integer | Book ID |

**Success Response** (200):
```json
{
  "id": 1,
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "price": 34.99,
  "description": "A handbook of agile software craftsmanship",
  "coverImageUrl": "https://example.com/cover.jpg",
  "category": "Technology",
  "stock": 20,
  "createdAt": "2024-01-15T10:30:00Z"
}
```

**Error Responses**:
- **404 Not Found**: Book not found

**Example**:
```bash
curl http://localhost:5000/api/books/1
```

---

## Cart Endpoints

### Get Cart

Retrieves all items in the authenticated user's cart.

**Endpoint**: `GET /api/cart`

**Authentication**: Required

**Success Response** (200):
```json
[
  {
    "id": 1,
    "userId": 5,
    "bookId": 1,
    "quantity": 2,
    "book": {
      "id": 1,
      "title": "Clean Code",
      "author": "Robert C. Martin",
      "price": 34.99
    }
  }
]
```

**Error Responses**:
- **401 Unauthorized**: Authentication required

**Example**:
```bash
curl http://localhost:5000/api/cart \
  -H "Authorization: Bearer <your-token>"
```

---

### Add to Cart

Adds a book to the cart or updates quantity if already exists.

**Endpoint**: `POST /api/cart`

**Authentication**: Required

**Request Body**:
```json
{
  "bookId": 1,
  "quantity": 2
}
```

**Request Fields**:
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| bookId | integer | Yes | ID of the book |
| quantity | integer | Yes | Number of items |

**Success Response** (200):
```json
{
  "message": "Item added to cart successfully"
}
```

**Error Responses**:
- **400 Bad Request**: Invalid data or book not available
- **401 Unauthorized**: Authentication required

**Example**:
```bash
curl -X POST http://localhost:5000/api/cart \
  -H "Authorization: Bearer <your-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "bookId": 1,
    "quantity": 2
  }'
```

---

### Update Cart Item Quantity

Updates the quantity of an item in the cart.

**Endpoint**: `PUT /api/cart/{bookId}`

**Authentication**: Required

**Path Parameters**:
| Parameter | Type | Description |
|-----------|------|-------------|
| bookId | integer | Book ID |

**Request Body**:
```json
{
  "quantity": 3
}
```

**Success Response** (200):
```json
{
  "message": "Cart updated successfully"
}
```

**Error Responses**:
- **404 Not Found**: Cart item not found
- **401 Unauthorized**: Authentication required

**Example**:
```bash
curl -X PUT http://localhost:5000/api/cart/1 \
  -H "Authorization: Bearer <your-token>" \
  -H "Content-Type: application/json" \
  -d '{"quantity": 3}'
```

---

### Remove from Cart

Removes an item from the cart.

**Endpoint**: `DELETE /api/cart/{bookId}`

**Authentication**: Required

**Path Parameters**:
| Parameter | Type | Description |
|-----------|------|-------------|
| bookId | integer | Book ID |

**Success Response** (200):
```json
{
  "message": "Item removed from cart"
}
```

**Error Responses**:
- **404 Not Found**: Cart item not found
- **401 Unauthorized**: Authentication required

**Example**:
```bash
curl -X DELETE http://localhost:5000/api/cart/1 \
  -H "Authorization: Bearer <your-token>"
```

---

### Checkout

Creates an order from cart items and clears the cart.

**Endpoint**: `POST /api/cart/checkout`

**Authentication**: Required

**Success Response** (200):
```json
{
  "orderId": 42,
  "message": "Order placed successfully"
}
```

**Error Responses**:
- **400 Bad Request**: Cart is empty
- **401 Unauthorized**: Authentication required

**Example**:
```bash
curl -X POST http://localhost:5000/api/cart/checkout \
  -H "Authorization: Bearer <your-token>"
```

---

## Review Endpoints

### Get Reviews for Book

Retrieves all reviews for a specific book.

**Endpoint**: `GET /api/reviews/book/{bookId}`

**Authentication**: Not required

**Path Parameters**:
| Parameter | Type | Description |
|-----------|------|-------------|
| bookId | integer | Book ID |

**Success Response** (200):
```json
[
  {
    "id": 1,
    "userId": 5,
    "bookId": 1,
    "rating": 5,
    "comment": "Excellent book! Highly recommended.",
    "createdAt": "2024-01-20T14:30:00Z",
    "username": "johndoe"
  }
]
```

**Example**:
```bash
curl http://localhost:5000/api/reviews/book/1
```

---

### Add Review

Creates a new review for a book.

**Endpoint**: `POST /api/reviews`

**Authentication**: Required

**Request Body**:
```json
{
  "bookId": 1,
  "rating": 5,
  "comment": "Excellent book! Highly recommended."
}
```

**Request Fields**:
| Field | Type | Required | Validation |
|-------|------|----------|------------|
| bookId | integer | Yes | - |
| rating | integer | Yes | Between 1 and 5 |
| comment | string | Yes | - |

**Success Response** (201):
```json
{
  "id": 1,
  "userId": 5,
  "bookId": 1,
  "rating": 5,
  "comment": "Excellent book! Highly recommended.",
  "createdAt": "2024-01-20T14:30:00Z"
}
```

**Error Responses**:
- **400 Bad Request**: User already reviewed this book
- **400 Bad Request**: Validation errors
- **401 Unauthorized**: Authentication required

**Example**:
```bash
curl -X POST http://localhost:5000/api/reviews \
  -H "Authorization: Bearer <your-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "bookId": 1,
    "rating": 5,
    "comment": "Excellent book!"
  }'
```

---

### Update Review

Updates an existing review.

**Endpoint**: `PUT /api/reviews/{id}`

**Authentication**: Required (must be review owner)

**Path Parameters**:
| Parameter | Type | Description |
|-----------|------|-------------|
| id | integer | Review ID |

**Request Body**:
```json
{
  "rating": 4,
  "comment": "Updated review comment"
}
```

**Success Response** (200):
```json
{
  "message": "Review updated successfully"
}
```

**Error Responses**:
- **403 Forbidden**: Not the review owner
- **404 Not Found**: Review not found
- **401 Unauthorized**: Authentication required

**Example**:
```bash
curl -X PUT http://localhost:5000/api/reviews/1 \
  -H "Authorization: Bearer <your-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "rating": 4,
    "comment": "Updated comment"
  }'
```

---

### Delete Review

Deletes a review.

**Endpoint**: `DELETE /api/reviews/{id}`

**Authentication**: Required (must be review owner or admin)

**Path Parameters**:
| Parameter | Type | Description |
|-----------|------|-------------|
| id | integer | Review ID |

**Success Response** (200):
```json
{
  "message": "Review deleted successfully"
}
```

**Error Responses**:
- **403 Forbidden**: Not the review owner
- **404 Not Found**: Review not found
- **401 Unauthorized**: Authentication required

**Example**:
```bash
curl -X DELETE http://localhost:5000/api/reviews/1 \
  -H "Authorization: Bearer <your-token>"
```

---

## Admin Endpoints

All admin endpoints require authentication and Admin role.

### Add Book

Adds a new book to the catalog.

**Endpoint**: `POST /api/admin/books`

**Authentication**: Required (Admin only)

**Request Body**:
```json
{
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "price": 34.99,
  "description": "A handbook of agile software craftsmanship",
  "coverImageUrl": "https://example.com/cover.jpg",
  "category": "Technology",
  "stock": 20
}
```

**Request Fields**:
| Field | Type | Required | Validation |
|-------|------|----------|------------|
| title | string | Yes | - |
| author | string | Yes | - |
| price | decimal | Yes | Between 0.01 and 99,999.99 |
| description | string | No | - |
| coverImageUrl | string | No | - |
| category | string | No | - |
| stock | integer | No | Non-negative |

**Success Response** (201):
```json
{
  "id": 10,
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "price": 34.99,
  "message": "Book created successfully"
}
```

**Error Responses**:
- **400 Bad Request**: Validation errors
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Admin role required

**Example**:
```bash
curl -X POST http://localhost:5000/api/admin/books \
  -H "Authorization: Bearer <admin-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Clean Code",
    "author": "Robert C. Martin",
    "price": 34.99,
    "stock": 20
  }'
```

---

### Update Book

Updates an existing book's information.

**Endpoint**: `PUT /api/admin/books/{id}`

**Authentication**: Required (Admin only)

**Path Parameters**:
| Parameter | Type | Description |
|-----------|------|-------------|
| id | integer | Book ID |

**Request Body**: Same as Add Book

**Success Response** (200):
```json
{
  "message": "Book updated successfully"
}
```

**Error Responses**:
- **404 Not Found**: Book not found
- **400 Bad Request**: Validation errors
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Admin role required

**Example**:
```bash
curl -X PUT http://localhost:5000/api/admin/books/1 \
  -H "Authorization: Bearer <admin-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Clean Code",
    "price": 29.99,
    "stock": 25
  }'
```

---

### Delete Book

Deletes a book from the catalog.

**Endpoint**: `DELETE /api/admin/books/{id}`

**Authentication**: Required (Admin only)

**Path Parameters**:
| Parameter | Type | Description |
|-----------|------|-------------|
| id | integer | Book ID |

**Success Response** (200):
```json
{
  "message": "Book deleted successfully"
}
```

**Error Responses**:
- **404 Not Found**: Book not found
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Admin role required

**Note**: Deleting a book cascades to cart_items and reviews.

**Example**:
```bash
curl -X DELETE http://localhost:5000/api/admin/books/1 \
  -H "Authorization: Bearer <admin-token>"
```

---

### Get All Orders

Retrieves all orders in the system.

**Endpoint**: `GET /api/admin/orders`

**Authentication**: Required (Admin only)

**Success Response** (200):
```json
[
  {
    "id": 1,
    "userId": 5,
    "totalAmount": 69.98,
    "status": "Pending",
    "createdAt": "2024-01-20T10:00:00Z",
    "items": [
      {
        "bookId": 1,
        "quantity": 2,
        "price": 34.99
      }
    ]
  }
]
```

**Error Responses**:
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Admin role required

**Example**:
```bash
curl http://localhost:5000/api/admin/orders \
  -H "Authorization: Bearer <admin-token>"
```

---

## Rate Limiting

Consider implementing rate limiting for:
- Auth endpoints: 5 requests per minute
- Public endpoints: 100 requests per minute
- Authenticated endpoints: 200 requests per minute

## CORS

The API allows requests from:
- `http://localhost:4200` (Angular dev server)

Configure additional origins in `Program.cs`.

## Pagination

Future versions will include pagination for endpoints returning large datasets:
```
GET /api/books?page=1&limit=20
```

## Postman Collection

Import the API into Postman for testing:

1. Open Postman
2. Click "Import"
3. Choose "Link" and enter: `http://localhost:5000/swagger/v1/swagger.json`
4. Or use Swagger UI to test endpoints interactively

## Code Examples

### JavaScript/Fetch
```javascript
// Login
const response = await fetch('http://localhost:5000/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'johndoe',
    password: 'SecurePass123!'
  })
});
const { token } = await response.json();

// Get books with token
const books = await fetch('http://localhost:5000/api/books', {
  headers: { 'Authorization': `Bearer ${token}` }
});
```

### Python/Requests
```python
import requests

# Login
response = requests.post('http://localhost:5000/api/auth/login', json={
    'username': 'johndoe',
    'password': 'SecurePass123!'
})
token = response.json()['token']

# Get cart
headers = {'Authorization': f'Bearer {token}'}
cart = requests.get('http://localhost:5000/api/cart', headers=headers)
```

### C#/HttpClient
```csharp
// Login
var client = new HttpClient();
var loginDto = new { username = "johndoe", password = "SecurePass123!" };
var response = await client.PostAsJsonAsync("http://localhost:5000/api/auth/login", loginDto);
var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

// Use token
client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", authResponse.Token);
var books = await client.GetFromJsonAsync<List<Book>>("http://localhost:5000/api/books");
```

## Support

For issues or questions about the API:
- Check Swagger documentation: `http://localhost:5000/swagger`
- Open an issue on GitHub
- Contact the development team
