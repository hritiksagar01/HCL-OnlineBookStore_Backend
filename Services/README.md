# Services

This directory contains the business logic services for the Online BookStore application.

## Overview

Services encapsulate business logic and coordinate between controllers and repositories. They handle complex operations, validation, and orchestration of multiple repository calls.

## Services

### 1. JwtService.cs

Handles JWT (JSON Web Token) generation for user authentication.

**Purpose**: Generate secure JWT tokens for authenticated users

**Dependencies**:
- `IConfiguration`: Access to JWT settings from appsettings.json

**Key Methods**:

#### `GenerateToken(string username, string role)`
Generates a JWT token for a user.

**Parameters**:
- `username` (string): The user's username
- `role` (string): The user's role ("User" or "Admin")

**Returns**: `string` - JWT token

**Token Claims**:
- `Subject`: Username
- `Role`: User role
- `JwtId`: Unique token identifier (GUID)
- `IssuedAt`: Token creation time

**Token Configuration** (from appsettings.json):
- `SecretKey`: Signing key (must be at least 32 characters)
- `Issuer`: Token issuer identifier
- `Audience`: Intended token audience
- `ExpiryInMinutes`: Token lifetime (default: 60 minutes)

**Example Usage**:
```csharp
var token = _jwtService.GenerateToken("johndoe", "User");
// Returns: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Security Features**:
- Uses HMAC-SHA256 algorithm
- Includes expiration time
- Includes unique token ID
- Cryptographically signed

**Configuration Example**:
```json
"JwtSettings": {
  "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!@#$",
  "Issuer": "OnlineBookStore",
  "Audience": "OnlineBookStoreUsers",
  "ExpiryInMinutes": 60
}
```

### 2. AuthService.cs

Handles user authentication and registration logic.

**Purpose**: Coordinate authentication operations including registration and login

**Dependencies**:
- `IUserRepository`: User data access
- `JwtService`: Token generation

**Key Methods**:

#### `RegisterAsync(RegisterDto dto)`
Registers a new user account.

**Parameters**:
- `dto` (RegisterDto): Registration data (username, email, password, confirmPassword)

**Returns**: `Task<AuthResponseDto?>` - Token and user info, or null if registration fails

**Process**:
1. Check if username or email already exists
2. Hash the password using BCrypt
3. Create new user with "User" role
4. Save to database
5. Generate JWT token
6. Return token and user information

**Validation**:
- Username must be unique
- Email must be unique
- Password requirements handled by DTO validation

**Example**:
```csharp
var result = await _authService.RegisterAsync(new RegisterDto
{
    Username = "johndoe",
    Email = "john@example.com",
    Password = "SecurePass123!",
    ConfirmPassword = "SecurePass123!"
});
// Returns: { Token: "...", Username: "johndoe", Role: "User" }
```

#### `LoginAsync(LoginDto dto)`
Authenticates a user and generates a token.

**Parameters**:
- `dto` (LoginDto): Login credentials (username, password)

**Returns**: `Task<AuthResponseDto?>` - Token and user info, or null if authentication fails

**Process**:
1. Find user by username
2. Verify password using BCrypt
3. Generate JWT token if credentials are valid
4. Return token and user information

**Security Features**:
- Password verification using BCrypt
- No information leakage (same error for invalid username or password)
- Secure password hashing comparison

**Example**:
```csharp
var result = await _authService.LoginAsync(new LoginDto
{
    Username = "johndoe",
    Password = "SecurePass123!"
});
// Returns: { Token: "...", Username: "johndoe", Role: "User" }
```

## Service Layer Benefits

### 1. Separation of Concerns
- Controllers handle HTTP concerns
- Services handle business logic
- Repositories handle data access

### 2. Reusability
Services can be used by multiple controllers or other services.

### 3. Testability
Business logic can be unit tested independently of HTTP context.

### 4. Transaction Management
Services can coordinate multiple repository operations.

### 5. Validation
Centralized business rule validation.

## Authentication Flow

### Registration Flow
```
User → Controller → AuthService → UserRepository → Database
                                ↓
                           JwtService
                                ↓
                              Token
```

### Login Flow
```
User → Controller → AuthService → UserRepository (find user)
                                ↓
                         BCrypt.Verify(password)
                                ↓
                           JwtService (if valid)
                                ↓
                              Token
```

## Security Best Practices

### Password Hashing
- Uses BCrypt with work factor 12
- Passwords never stored in plain text
- Hash verification is timing-attack resistant

### JWT Security
- Tokens are signed with HMAC-SHA256
- Include expiration time
- Include role claims for authorization
- Secret key stored in configuration (move to environment variables in production)

### Error Handling
- Don't reveal whether username or email exists during registration
- Don't reveal whether username or password is incorrect during login
- Use generic error messages

## Configuration

Services rely on configuration from `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!@#$",
    "Issuer": "OnlineBookStore",
    "Audience": "OnlineBookStoreUsers",
    "ExpiryInMinutes": 60
  }
}
```

**Production Considerations**:
- Store `SecretKey` in environment variables or Key Vault
- Use longer expiry times for refresh tokens
- Implement token refresh mechanism
- Add rate limiting for auth endpoints

## Adding New Services

When adding a new service:

1. Create interface (optional but recommended)
2. Implement service class
3. Register in `Program.cs` dependency injection
4. Add unit tests
5. Document in this README

**Example Service**:

```csharp
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // Email sending logic
    }
}
```

**Registration**:
```csharp
builder.Services.AddScoped<IEmailService, EmailService>();
```

## Future Service Enhancements

Potential services to add:

1. **EmailService**: Send notifications and verification emails
2. **OrderService**: Handle complex order operations
3. **PaymentService**: Process payments
4. **NotificationService**: Send notifications
5. **CacheService**: Manage caching
6. **FileService**: Handle file uploads
7. **RecommendationService**: Book recommendations
8. **SearchService**: Advanced search functionality

## Testing Services

Services should be unit tested with mocked dependencies:

```csharp
[Fact]
public async Task RegisterAsync_WithValidData_ReturnsAuthResponse()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    var mockJwt = new Mock<JwtService>();
    var authService = new AuthService(mockRepo.Object, mockJwt.Object);

    // Act
    var result = await authService.RegisterAsync(validDto);

    // Assert
    Assert.NotNull(result);
    Assert.NotNull(result.Token);
}
```

## Error Handling

Services should:
- Return `null` for expected failure cases (e.g., invalid credentials)
- Throw exceptions for unexpected errors
- Log errors appropriately
- Not expose sensitive information in error messages
