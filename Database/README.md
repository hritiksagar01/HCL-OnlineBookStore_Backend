# Database

This directory contains database-related files including database schema, initialization scripts, and database helper utilities.

## Overview

The Online BookStore uses **PostgreSQL** as its database management system. This directory provides everything needed to set up and manage the database.

## Files

### 1. init.sql

Database initialization script that creates all tables and seeds initial data.

**Purpose**: Set up the complete database schema from scratch

**Contents**:
1. Table creation statements
2. Constraints and indexes
3. Seed data (admin user and sample books)

**Usage**:
```bash
psql -U postgres -d bookstore_db -f Database/init.sql
```

#### Database Schema

**Tables Created**:

##### users
Stores user account information.

```sql
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    role VARCHAR(20) DEFAULT 'User',
    created_at TIMESTAMP DEFAULT NOW()
);
```

**Columns**:
- `id`: Auto-incrementing primary key
- `username`: Unique username (max 100 chars)
- `email`: Unique email address (max 255 chars)
- `password_hash`: BCrypt hashed password
- `role`: User role ("User" or "Admin")
- `created_at`: Account creation timestamp

**Constraints**:
- UNIQUE on username
- UNIQUE on email

---

##### books
Stores book catalog information.

```sql
CREATE TABLE IF NOT EXISTS books (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    author VARCHAR(255) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    description TEXT,
    cover_image_url TEXT,
    category VARCHAR(100),
    stock INT DEFAULT 0,
    created_at TIMESTAMP DEFAULT NOW()
);
```

**Columns**:
- `id`: Auto-incrementing primary key
- `title`: Book title
- `author`: Author name
- `price`: Book price (10,2 precision)
- `description`: Book description (optional)
- `cover_image_url`: URL to cover image (optional)
- `category`: Book category/genre (optional)
- `stock`: Available inventory
- `created_at`: Record creation timestamp

---

##### cart_items
Stores user shopping cart items.

```sql
CREATE TABLE IF NOT EXISTS cart_items (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES users(id) ON DELETE CASCADE,
    book_id INT REFERENCES books(id) ON DELETE CASCADE,
    quantity INT DEFAULT 1,
    UNIQUE(user_id, book_id)
);
```

**Columns**:
- `id`: Auto-incrementing primary key
- `user_id`: Foreign key to users
- `book_id`: Foreign key to books
- `quantity`: Number of books

**Constraints**:
- FOREIGN KEY to users (CASCADE delete)
- FOREIGN KEY to books (CASCADE delete)
- UNIQUE constraint on (user_id, book_id) - prevents duplicate items

---

##### reviews
Stores book reviews and ratings.

```sql
CREATE TABLE IF NOT EXISTS reviews (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES users(id) ON DELETE CASCADE,
    book_id INT REFERENCES books(id) ON DELETE CASCADE,
    rating INT CHECK (rating BETWEEN 1 AND 5),
    comment TEXT,
    created_at TIMESTAMP DEFAULT NOW(),
    UNIQUE(user_id, book_id)
);
```

**Columns**:
- `id`: Auto-incrementing primary key
- `user_id`: Foreign key to users
- `book_id`: Foreign key to books
- `rating`: Rating from 1 to 5 stars
- `comment`: Review text
- `created_at`: Review timestamp

**Constraints**:
- FOREIGN KEY to users (CASCADE delete)
- FOREIGN KEY to books (CASCADE delete)
- CHECK constraint: rating BETWEEN 1 AND 5
- UNIQUE constraint on (user_id, book_id) - one review per user per book

---

##### orders
Stores customer orders.

```sql
CREATE TABLE IF NOT EXISTS orders (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES users(id),
    total_amount DECIMAL(10,2),
    status VARCHAR(50) DEFAULT 'Pending',
    created_at TIMESTAMP DEFAULT NOW()
);
```

**Columns**:
- `id`: Auto-incrementing primary key
- `user_id`: Foreign key to users
- `total_amount`: Total order cost (10,2 precision)
- `status`: Order status (default: "Pending")
- `created_at`: Order creation timestamp

**Status Values**:
- "Pending": Order placed, awaiting processing
- "Processing": Order being prepared
- "Shipped": Order dispatched
- "Delivered": Order completed
- "Cancelled": Order cancelled

---

##### order_items
Stores individual items within orders.

```sql
CREATE TABLE IF NOT EXISTS order_items (
    id SERIAL PRIMARY KEY,
    order_id INT REFERENCES orders(id) ON DELETE CASCADE,
    book_id INT REFERENCES books(id),
    quantity INT,
    price DECIMAL(10,2)
);
```

**Columns**:
- `id`: Auto-incrementing primary key
- `order_id`: Foreign key to orders
- `book_id`: Foreign key to books
- `quantity`: Number of books ordered
- `price`: Price per book at time of order

**Constraints**:
- FOREIGN KEY to orders (CASCADE delete)
- FOREIGN KEY to books

**Note**: Price is stored to maintain historical accuracy even if book price changes.

---

#### Seed Data

The initialization script includes seed data:

##### Default Admin User
```sql
INSERT INTO users (username, email, password_hash, role)
VALUES ('admin', 'admin@bookstore.com', '$2a$12$...', 'Admin')
ON CONFLICT (username) DO NOTHING;
```

**Credentials**:
- Username: `admin`
- Password: `Admin@123` (hashed with BCrypt)
- Role: Admin

**Important**: Change the admin password after first login!

##### Sample Books
The script seeds 8 sample books across different categories:
- Technology (Clean Code, Design Patterns)
- Fiction (The Catcher in the Rye, To Kill a Mockingbird, The Great Gatsby)
- Fantasy (The Hobbit)
- Romance (Pride and Prejudice)
- Science Fiction (1984)

---

### 2. DbHelper.cs

Database connection helper class.

**Purpose**: Centralize database connection creation and management

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

**Features**:
- Reads connection string from configuration
- Creates new PostgreSQL connections on demand
- Registered as Scoped service in DI container

**Registration** (in Program.cs):
```csharp
builder.Services.AddScoped<DbHelper>();
```

**Usage** (in repositories):
```csharp
public class BookRepository
{
    private readonly DbHelper _dbHelper;

    public BookRepository(DbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        using var connection = _dbHelper.GetConnection();
        await connection.OpenAsync();
        // Execute query...
    }
}
```

## Database Setup

### Prerequisites
- PostgreSQL 14 or later installed
- PostgreSQL running on default port (5432)

### Setup Steps

1. **Create Database**
   ```bash
   psql -U postgres
   CREATE DATABASE bookstore_db;
   \q
   ```

2. **Run Initialization Script**
   ```bash
   psql -U postgres -d bookstore_db -f Database/init.sql
   ```

3. **Verify Setup**
   ```bash
   psql -U postgres -d bookstore_db
   \dt              # List all tables
   \d users         # Describe users table
   SELECT * FROM users;  # View admin user
   \q
   ```

4. **Update Connection String**

   Edit `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=bookstore_db;Username=postgres;Password=your_password"
   }
   ```

## Database Relationships

```
┌──────────┐
│  users   │
└────┬─────┘
     │
     ├─────────┐
     │         │
     ▼         ▼
┌──────────┐ ┌─────────┐
│cart_items│ │ orders  │
└────┬─────┘ └────┬────┘
     │            │
     │            ▼
     │       ┌───────────┐
     │       │order_items│
     │       └─────┬─────┘
     │             │
     │             │
┌────▼─────────────▼─┐
│      books         │
└────────┬───────────┘
         │
         ▼
    ┌─────────┐
    │ reviews │
    └─────────┘
```

## Database Migrations

Currently, the project uses SQL scripts for schema management. For production:

### Recommended Migration Strategy

1. **Version Control**: Keep all schema changes in versioned SQL files
   ```
   Database/
   ├── init.sql (v1.0.0)
   ├── migrations/
   │   ├── 001_add_wishlist_table.sql
   │   ├── 002_add_book_ratings.sql
   │   └── ...
   ```

2. **Migration Tool**: Consider using:
   - **FluentMigrator**: .NET migration framework
   - **DbUp**: Simple migration tool
   - **Entity Framework Core Migrations**: If switching to EF Core

3. **Migration Pattern**:
   ```sql
   -- Migration: 001_add_wishlist_table.sql
   -- Description: Add wishlist feature
   -- Date: 2024-01-15

   CREATE TABLE IF NOT EXISTS wishlist (
       id SERIAL PRIMARY KEY,
       user_id INT REFERENCES users(id) ON DELETE CASCADE,
       book_id INT REFERENCES books(id) ON DELETE CASCADE,
       created_at TIMESTAMP DEFAULT NOW(),
       UNIQUE(user_id, book_id)
   );
   ```

## Backup and Restore

### Backup Database
```bash
# Full backup
pg_dump -U postgres bookstore_db > backup.sql

# Data only
pg_dump -U postgres --data-only bookstore_db > data_backup.sql

# Schema only
pg_dump -U postgres --schema-only bookstore_db > schema_backup.sql
```

### Restore Database
```bash
# Drop and recreate
psql -U postgres -c "DROP DATABASE IF EXISTS bookstore_db;"
psql -U postgres -c "CREATE DATABASE bookstore_db;"

# Restore
psql -U postgres -d bookstore_db < backup.sql
```

## Performance Optimization

### Indexes

Consider adding indexes for frequently queried columns:

```sql
-- Index on book title for search
CREATE INDEX idx_books_title ON books(title);

-- Index on book category for filtering
CREATE INDEX idx_books_category ON books(category);

-- Index on order status for filtering
CREATE INDEX idx_orders_status ON orders(status);

-- Index on review book_id for fetching reviews
CREATE INDEX idx_reviews_book_id ON reviews(book_id);
```

### Query Optimization Tips

1. **Use EXPLAIN**: Analyze query performance
   ```sql
   EXPLAIN ANALYZE SELECT * FROM books WHERE category = 'Technology';
   ```

2. **Limit Results**: Use LIMIT for large result sets
3. **Avoid N+1 Queries**: Join related tables instead of multiple queries
4. **Use Connection Pooling**: Configure in connection string

## Security Best Practices

1. **Connection String Security**
   - Store in environment variables
   - Use Azure Key Vault or AWS Secrets Manager in production
   - Never commit passwords to version control

2. **Database User Permissions**
   ```sql
   -- Create application user with limited permissions
   CREATE USER bookstore_app WITH PASSWORD 'secure_password';
   GRANT CONNECT ON DATABASE bookstore_db TO bookstore_app;
   GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO bookstore_app;
   GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO bookstore_app;
   ```

3. **SSL Connection**
   ```json
   "DefaultConnection": "Host=localhost;Database=bookstore_db;Username=bookstore_app;Password=secure_password;SslMode=Require"
   ```

4. **SQL Injection Prevention**
   - Always use parameterized queries
   - Never concatenate user input into SQL
   - Validate and sanitize all inputs

## Monitoring and Maintenance

### Regular Maintenance Tasks

1. **Vacuum Database**
   ```sql
   VACUUM ANALYZE;
   ```

2. **Check Table Sizes**
   ```sql
   SELECT
       schemaname,
       tablename,
       pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
   FROM pg_tables
   WHERE schemaname = 'public'
   ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;
   ```

3. **Monitor Slow Queries**
   ```sql
   -- Enable logging in postgresql.conf
   log_min_duration_statement = 1000  # Log queries taking > 1s
   ```

## Troubleshooting

### Common Issues

1. **Connection Failed**
   - Check PostgreSQL is running: `sudo systemctl status postgresql`
   - Verify connection string
   - Check firewall settings

2. **Permission Denied**
   - Verify database user has correct permissions
   - Check `pg_hba.conf` authentication settings

3. **Tables Not Found**
   - Ensure init.sql ran successfully
   - Check you're connected to correct database

4. **Constraint Violations**
   - Review unique constraints
   - Check foreign key relationships
   - Validate data before insert

## Future Enhancements

1. **Full-Text Search**: Add PostgreSQL full-text search for books
2. **Stored Procedures**: Move complex logic to database
3. **Views**: Create views for common queries
4. **Partitioning**: Partition large tables (orders) by date
5. **Replication**: Set up master-slave replication
6. **Connection Pooling**: Implement pgBouncer
