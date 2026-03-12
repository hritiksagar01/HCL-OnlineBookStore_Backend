# DTOs (Data Transfer Objects)

This directory contains Data Transfer Objects used for API request and response payloads.

## Overview

DTOs are lightweight objects designed for transferring data between the client and server. They provide:
- **Input Validation**: Using data annotations
- **API Contract**: Clear definition of expected data structure
- **Security**: Prevent over-posting and expose only necessary data
- **Decoupling**: Separate API models from domain models

## DTOs

### 1. RegisterDto.cs

Used for user registration requests.

**Purpose**: Capture user registration information

**Properties**:
```csharp
public class RegisterDto
{
    [Required]
    [MinLength(3)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}
```

**Validation Rules**:
- `Username`: Required, minimum 3 characters
- `Email`: Required, valid email format
- `Password`: Required, minimum 6 characters

**Usage**:
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Response**: `AuthResponseDto`

---

### 2. LoginDto.cs

Used for user login requests.

**Purpose**: Capture login credentials

**Properties**:
```csharp
public class LoginDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
```

**Validation Rules**:
- `Username`: Required
- `Password`: Required

**Usage**:
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "johndoe",
  "password": "SecurePass123!"
}
```

**Response**: `AuthResponseDto`

---

### 3. AuthResponseDto.cs

Used for authentication response (register/login).

**Purpose**: Return authentication token and user information

**Properties**:
```csharp
public class AuthResponseDto
{
    public string Token { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
}
```

**Fields**:
- `Token`: JWT authentication token
- `Username`: User's username
- `Role`: User's role ("User" or "Admin")

**Example Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "johndoe",
  "role": "User"
}
```

**Usage**: Client stores token and includes in Authorization header for subsequent requests

---

### 4. BookDto.cs

Used for creating and updating books (admin operations).

**Purpose**: Transfer book information for CRUD operations

**Properties**:
```csharp
public class BookDto
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Author { get; set; }

    [Required]
    [Range(0.01, 99999.99)]
    public decimal Price { get; set; }

    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Category { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}
```

**Validation Rules**:
- `Title`: Required
- `Author`: Required
- `Price`: Required, between 0.01 and 99,999.99
- `Description`: Optional
- `CoverImageUrl`: Optional
- `Category`: Optional
- `Stock`: Optional, non-negative

**Usage (Create Book)**:
```http
POST /api/admin/books
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "price": 34.99,
  "description": "A handbook of agile software craftsmanship",
  "category": "Technology",
  "stock": 20
}
```

**Usage (Update Book)**:
```http
PUT /api/admin/books/5
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "price": 29.99,
  "stock": 25
}
```

---

### 5. CartItemDto.cs

Used for cart operations.

**Purpose**: Transfer cart item information

**Properties**:
```csharp
public class CartItemDto
{
    public int BookId { get; set; }
    public int Quantity { get; set; }
}
```

**Fields**:
- `BookId`: ID of the book to add/update
- `Quantity`: Number of items

**Usage (Add to Cart)**:
```http
POST /api/cart
Authorization: Bearer {token}
Content-Type: application/json

{
  "bookId": 5,
  "quantity": 2
}
```

**Usage (Update Quantity)**:
```http
PUT /api/cart/5
Authorization: Bearer {token}
Content-Type: application/json

{
  "quantity": 3
}
```

---

### 6. ReviewDto.cs

Used for creating and updating reviews.

**Purpose**: Transfer review information

**Properties**:
```csharp
public class ReviewDto
{
    public int BookId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    public string Comment { get; set; }
}
```

**Validation Rules**:
- `BookId`: Book being reviewed
- `Rating`: Required, between 1 and 5
- `Comment`: Review text

**Usage (Create Review)**:
```http
POST /api/reviews
Authorization: Bearer {token}
Content-Type: application/json

{
  "bookId": 5,
  "rating": 5,
  "comment": "Excellent book! Highly recommended for all developers."
}
```

**Usage (Update Review)**:
```http
PUT /api/reviews/10
Authorization: Bearer {token}
Content-Type: application/json

{
  "rating": 4,
  "comment": "Great book with practical examples."
}
```

## DTO Benefits

### 1. Input Validation
DTOs use data annotations for automatic validation:
```csharp
[Required]
[EmailAddress]
public string Email { get; set; }
```

Controller automatically validates:
```csharp
public IActionResult Register([FromBody] RegisterDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    // Process valid data
}
```

### 2. Security

**Prevent Over-Posting**:
```csharp
// Without DTO - vulnerable to over-posting
public IActionResult Update([FromBody] User user)
{
    // User could set Role, Id, or other sensitive fields
}

// With DTO - safe
public IActionResult Update([FromBody] BookDto dto)
{
    // Only specified fields can be set
}
```

**Prevent Information Disclosure**:
```csharp
// Don't expose password hash
public class User
{
    public string PasswordHash { get; set; } // Sensitive!
}

// Use DTO instead
public class AuthResponseDto
{
    // Only expose safe information
    public string Token { get; set; }
    public string Username { get; set; }
}
```

### 3. API Versioning
DTOs allow API evolution without changing domain models:
```csharp
// V1
public class BookDtoV1
{
    public string Title { get; set; }
}

// V2 - Add new fields without breaking V1 clients
public class BookDtoV2
{
    public string Title { get; set; }
    public string ISBN { get; set; } // New field
}
```

### 4. Clear API Contract
DTOs document the expected API payload structure:
```csharp
// Clear requirements for registration
public class RegisterDto
{
    [Required]
    [MinLength(3)]
    public string Username { get; set; }
}
```

## Data Annotations

### Common Validation Attributes

#### Required
```csharp
[Required]
public string Username { get; set; }
```
- Field must be provided
- Cannot be null or empty string

#### MinLength / MaxLength
```csharp
[MinLength(3)]
[MaxLength(100)]
public string Username { get; set; }
```
- String length constraints

#### Range
```csharp
[Range(1, 5)]
public int Rating { get; set; }

[Range(0.01, 99999.99)]
public decimal Price { get; set; }
```
- Numeric value must be within range

#### EmailAddress
```csharp
[EmailAddress]
public string Email { get; set; }
```
- Validates email format

#### RegularExpression
```csharp
[RegularExpression(@"^[a-zA-Z0-9]*$")]
public string Username { get; set; }
```
- Custom pattern validation

#### Compare
```csharp
[Compare("Password")]
public string ConfirmPassword { get; set; }
```
- Field must match another field

### Custom Error Messages
```csharp
[Required(ErrorMessage = "Username is required")]
[MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
public string Username { get; set; }
```

## Best Practices

### 1. Keep DTOs Simple
DTOs should be simple data containers without logic:
```csharp
// Good
public class BookDto
{
    public string Title { get; set; }
    public decimal Price { get; set; }
}

// Bad - contains logic
public class BookDto
{
    public string Title { get; set; }
    public decimal Price { get; set; }

    public decimal CalculateDiscount() // Don't do this
    {
        return Price * 0.1m;
    }
}
```

### 2. Use Nullable for Optional Fields
```csharp
public class BookDto
{
    [Required]
    public string Title { get; set; }

    public string? Description { get; set; } // Optional
}
```

### 3. Separate Input and Output DTOs
```csharp
// Input
public class CreateBookDto
{
    public string Title { get; set; }
    public decimal Price { get; set; }
}

// Output
public class BookResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### 4. Use AutoMapper for Mapping
Consider using AutoMapper to map between DTOs and models:
```csharp
var book = _mapper.Map<Book>(bookDto);
```

### 5. Document with XML Comments
```csharp
/// <summary>
/// Data transfer object for user registration
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// Username (3-100 characters)
    /// </summary>
    [Required]
    [MinLength(3)]
    public string Username { get; set; }
}
```

## Validation Error Responses

When validation fails, ASP.NET Core returns:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Username": [
      "The Username field is required."
    ],
    "Password": [
      "The Password field is required.",
      "The field Password must be a string with a minimum length of 6."
    ]
  }
}
```

## Adding New DTOs

When adding a new DTO:

1. Create class in this directory
2. Add data annotations for validation
3. Initialize strings to `string.Empty`
4. Use nullable types for optional fields
5. Document with XML comments
6. Update this README

**Example**:
```csharp
using System.ComponentModel.DataAnnotations;

namespace Online_BookStore__System.DTOs;

/// <summary>
/// DTO for password reset requests
/// </summary>
public class PasswordResetDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    public string ResetToken { get; set; } = string.Empty;
}
```

## Testing DTOs

DTOs should be tested for validation rules:

```csharp
[Fact]
public void RegisterDto_WithInvalidEmail_FailsValidation()
{
    // Arrange
    var dto = new RegisterDto
    {
        Username = "johndoe",
        Email = "invalid-email",
        Password = "password123"
    };

    // Act
    var validationResults = ValidateModel(dto);

    // Assert
    Assert.Contains(validationResults,
        v => v.MemberNames.Contains("Email"));
}
```

## Future Enhancements

1. **Response DTOs**: Separate DTOs for responses vs requests
2. **AutoMapper Integration**: Automatic mapping between DTOs and models
3. **FluentValidation**: More complex validation scenarios
4. **API Versioning**: Version-specific DTOs (V1, V2)
5. **Swagger Documentation**: Enhanced API documentation from DTOs
