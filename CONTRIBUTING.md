# Contributing to HCL Online BookStore Backend

Thank you for your interest in contributing to the HCL Online BookStore Backend project! This document provides guidelines and instructions for contributing.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Commit Guidelines](#commit-guidelines)
- [Pull Request Process](#pull-request-process)
- [Testing](#testing)
- [Documentation](#documentation)
- [Issue Reporting](#issue-reporting)

## Code of Conduct

### Our Standards

- Be respectful and inclusive
- Welcome newcomers and help them learn
- Focus on what is best for the community
- Show empathy towards others
- Accept constructive criticism gracefully

### Unacceptable Behavior

- Harassment, discrimination, or offensive comments
- Trolling or insulting/derogatory remarks
- Publishing others' private information
- Any conduct that could be considered inappropriate in a professional setting

## Getting Started

### Prerequisites

Before contributing, ensure you have:
- .NET 8.0 SDK or later
- PostgreSQL 14+
- Git
- A code editor (Visual Studio, VS Code, or Rider)
- Basic understanding of C#, ASP.NET Core, and PostgreSQL

### Setting Up Development Environment

1. **Fork the repository**
   ```bash
   # Click "Fork" on GitHub, then clone your fork
   git clone https://github.com/YOUR-USERNAME/HCL-OnlineBookStore_Backend.git
   cd HCL-OnlineBookStore_Backend
   ```

2. **Add upstream remote**
   ```bash
   git remote add upstream https://github.com/hritiksagar01/HCL-OnlineBookStore_Backend.git
   ```

3. **Set up the database**
   ```bash
   psql -U postgres -c "CREATE DATABASE bookstore_db;"
   psql -U postgres -d bookstore_db -f Database/init.sql
   ```

4. **Configure application**
   - Update `appsettings.json` with your database credentials
   - Change JWT secret key for development

5. **Build and run**
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

6. **Verify setup**
   - Visit `http://localhost:5000/swagger`
   - Test authentication endpoints

## Development Workflow

### Branch Naming Convention

Use descriptive branch names with prefixes:

- `feature/` - New features
  - Example: `feature/add-wishlist`
- `bugfix/` - Bug fixes
  - Example: `bugfix/fix-cart-quantity`
- `hotfix/` - Urgent production fixes
  - Example: `hotfix/security-patch`
- `refactor/` - Code refactoring
  - Example: `refactor/repository-pattern`
- `docs/` - Documentation updates
  - Example: `docs/api-documentation`

### Workflow Steps

1. **Create a branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make changes**
   - Write clean, maintainable code
   - Follow coding standards
   - Add tests for new functionality

3. **Commit changes**
   ```bash
   git add .
   git commit -m "feat: add wishlist feature"
   ```

4. **Keep your branch updated**
   ```bash
   git fetch upstream
   git rebase upstream/main
   ```

5. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Create Pull Request**
   - Go to GitHub and create a PR from your fork
   - Fill in the PR template
   - Link related issues

## Coding Standards

### C# Style Guide

Follow Microsoft's C# coding conventions:

#### Naming Conventions

```csharp
// Classes, Methods, Properties - PascalCase
public class BookService
{
    public string Title { get; set; }

    public void GetBooks() { }
}

// Private fields - _camelCase with underscore
private readonly IBookRepository _bookRepository;

// Parameters, local variables - camelCase
public void AddBook(Book book)
{
    var bookId = book.Id;
}

// Constants - PascalCase
private const int MaxRetries = 3;

// Interfaces - IPascalCase
public interface IBookRepository { }
```

#### Code Organization

```csharp
// 1. Using statements
using System;
using System.Linq;

// 2. Namespace
namespace Online_BookStore__System.Services;

// 3. Class with summary
/// <summary>
/// Service for managing book operations
/// </summary>
public class BookService
{
    // 4. Private fields
    private readonly IBookRepository _repository;

    // 5. Constructor
    public BookService(IBookRepository repository)
    {
        _repository = repository;
    }

    // 6. Public methods
    public async Task<Book> GetBookAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    // 7. Private methods
    private void ValidateBook(Book book)
    {
        // Validation logic
    }
}
```

#### Best Practices

1. **Use async/await consistently**
   ```csharp
   // Good
   public async Task<Book> GetBookAsync(int id)
   {
       return await _repository.GetByIdAsync(id);
   }

   // Bad
   public Book GetBook(int id)
   {
       return _repository.GetByIdAsync(id).Result; // Avoid .Result
   }
   ```

2. **Use meaningful variable names**
   ```csharp
   // Good
   var activeBooks = books.Where(b => b.Stock > 0);

   // Bad
   var b = books.Where(x => x.Stock > 0);
   ```

3. **Keep methods small and focused**
   ```csharp
   // Good - Single responsibility
   public async Task<Book> GetBookAsync(int id)
   {
       return await _repository.GetByIdAsync(id);
   }

   public async Task<bool> ValidateBookAsync(Book book)
   {
       return book.Stock >= 0 && book.Price > 0;
   }
   ```

4. **Use nullable reference types**
   ```csharp
   public string Title { get; set; } = string.Empty; // Required
   public string? Description { get; set; } // Optional
   ```

5. **Proper error handling**
   ```csharp
   try
   {
       var book = await _repository.GetByIdAsync(id);
       return Ok(book);
   }
   catch (Exception ex)
   {
       _logger.LogError(ex, "Error getting book {Id}", id);
       return StatusCode(500, "Internal server error");
   }
   ```

### SQL Style Guide

```sql
-- Use uppercase for SQL keywords
SELECT id, title, author
FROM books
WHERE category = 'Technology'
ORDER BY created_at DESC;

-- Proper indentation
CREATE TABLE IF NOT EXISTS books (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    author VARCHAR(255) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    created_at TIMESTAMP DEFAULT NOW()
);
```

### File Organization

- One class per file
- File name matches class name
- Organize using folders (Controllers, Models, Services, etc.)
- Keep related files together

## Commit Guidelines

### Commit Message Format

Use [Conventional Commits](https://www.conventionalcommits.org/) format:

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, no logic change)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

### Examples

```bash
# Feature
git commit -m "feat(cart): add ability to save cart for later"

# Bug fix
git commit -m "fix(auth): resolve token expiration issue"

# Documentation
git commit -m "docs(readme): update installation instructions"

# Breaking change
git commit -m "feat(api): change book endpoint response format

BREAKING CHANGE: Book endpoint now returns BookDto instead of Book model"
```

### Commit Best Practices

- Write clear, descriptive commit messages
- Keep commits atomic (one logical change per commit)
- Commit working code (don't break the build)
- Don't commit commented-out code
- Don't commit sensitive information (passwords, keys)

## Pull Request Process

### Before Submitting

1. **Update your branch**
   ```bash
   git fetch upstream
   git rebase upstream/main
   ```

2. **Run tests**
   ```bash
   dotnet test
   ```

3. **Check code quality**
   ```bash
   dotnet build
   ```

4. **Update documentation** if needed

### PR Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Related Issues
Closes #123

## Testing
- [ ] Added unit tests
- [ ] Tested manually
- [ ] All tests pass

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-reviewed code
- [ ] Commented complex code
- [ ] Updated documentation
- [ ] No new warnings
```

### Review Process

1. Maintainers will review your PR
2. Address review comments
3. Push updates to your branch
4. PR will be merged once approved

### After Merge

1. Delete your feature branch
   ```bash
   git branch -d feature/your-feature-name
   git push origin --delete feature/your-feature-name
   ```

2. Update your local main
   ```bash
   git checkout main
   git pull upstream main
   ```

## Testing

### Unit Tests

Write unit tests for:
- Service methods
- Repository methods
- Business logic
- Validation logic

```csharp
[Fact]
public async Task GetBookAsync_WithValidId_ReturnsBook()
{
    // Arrange
    var mockRepo = new Mock<IBookRepository>();
    mockRepo.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new Book { Id = 1, Title = "Test" });
    var service = new BookService(mockRepo.Object);

    // Act
    var result = await service.GetBookAsync(1);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test", result.Title);
}
```

### Integration Tests

Test API endpoints with a test database:

```csharp
[Fact]
public async Task GetBooks_ReturnsOkWithBooks()
{
    // Arrange
    var client = _factory.CreateClient();

    // Act
    var response = await client.GetAsync("/api/books");

    // Assert
    response.EnsureSuccessStatusCode();
    var books = await response.Content.ReadFromJsonAsync<List<Book>>();
    Assert.NotEmpty(books);
}
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific test
dotnet test --filter "FullyQualifiedName~BookServiceTests"
```

## Documentation

### Code Documentation

Use XML documentation comments:

```csharp
/// <summary>
/// Retrieves a book by its unique identifier
/// </summary>
/// <param name="id">The book ID</param>
/// <returns>The book if found, null otherwise</returns>
/// <exception cref="ArgumentException">Thrown when id is invalid</exception>
public async Task<Book?> GetBookAsync(int id)
{
    // Implementation
}
```

### README Updates

When adding features:
1. Update main README.md
2. Update relevant component README
3. Update API_DOCUMENTATION.md if API changes
4. Add examples where helpful

### API Documentation

Update Swagger documentation:

```csharp
[HttpGet("{id}")]
[ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetBook(int id)
{
    // Implementation
}
```

## Issue Reporting

### Bug Reports

Include:
- Clear, descriptive title
- Steps to reproduce
- Expected behavior
- Actual behavior
- Environment (OS, .NET version, PostgreSQL version)
- Error messages/logs
- Screenshots if applicable

### Feature Requests

Include:
- Clear description of the feature
- Use case/motivation
- Proposed solution
- Alternative solutions considered
- Additional context

### Issue Labels

- `bug` - Something isn't working
- `enhancement` - New feature or request
- `documentation` - Documentation improvements
- `good first issue` - Good for newcomers
- `help wanted` - Extra attention needed
- `question` - Further information requested

## Questions?

- Check existing documentation
- Search closed issues
- Ask in GitHub Discussions
- Contact maintainers

## License

By contributing, you agree that your contributions will be licensed under the same license as the project.

---

**Thank you for contributing to HCL Online BookStore Backend!**
