-- Online Bookstore System - PostgreSQL Database Schema
ON CONFLICT DO NOTHING;
('The Hobbit', 'J.R.R. Tolkien', 13.99, 'A fantasy novel about the adventures of Bilbo Baggins.', 'https://covers.openlibrary.org/b/id/8228691-L.jpg', 'Fantasy', 45)
('Design Patterns', 'Gang of Four', 44.99, 'Elements of reusable object-oriented software.', 'https://covers.openlibrary.org/b/id/7222246-L.jpg', 'Technology', 15),
('Clean Code', 'Robert C. Martin', 34.99, 'A handbook of agile software craftsmanship.', 'https://covers.openlibrary.org/b/id/8228691-L.jpg', 'Technology', 20),
('The Catcher in the Rye', 'J.D. Salinger', 10.99, 'A story about teenage alienation and loss of innocence.', 'https://covers.openlibrary.org/b/id/7222246-L.jpg', 'Fiction', 30),
('Pride and Prejudice', 'Jane Austen', 9.99, 'A romantic novel about the Bennet family and their five unmarried daughters.', 'https://covers.openlibrary.org/b/id/8228691-L.jpg', 'Romance', 25),
('1984', 'George Orwell', 11.99, 'A dystopian novel set in a totalitarian society ruled by Big Brother.', 'https://covers.openlibrary.org/b/id/7222246-L.jpg', 'Science Fiction', 40),
('To Kill a Mockingbird', 'Harper Lee', 14.99, 'A novel about racial injustice in the Deep South, seen through the eyes of a young girl.', 'https://covers.openlibrary.org/b/id/8228691-L.jpg', 'Fiction', 35),
('The Great Gatsby', 'F. Scott Fitzgerald', 12.99, 'A story of the mysteriously wealthy Jay Gatsby and his love for Daisy Buchanan.', 'https://covers.openlibrary.org/b/id/7222246-L.jpg', 'Fiction', 50),
INSERT INTO books (title, author, price, description, cover_image_url, category, stock) VALUES
-- Seed sample books

ON CONFLICT (username) DO NOTHING;
VALUES ('admin', 'admin@bookstore.com', '$2a$12$LJ3m4ys3uz0CF5TXHKPSHO0pOmWNMwnMp2VOLOVPaGHBmSmxEPOuO', 'Admin')
INSERT INTO users (username, email, password_hash, role)
-- Seed admin user (password: Admin@123)

);
    price DECIMAL(10,2)
    quantity INT,
    book_id INT REFERENCES books(id),
    order_id INT REFERENCES orders(id) ON DELETE CASCADE,
    id SERIAL PRIMARY KEY,
CREATE TABLE IF NOT EXISTS order_items (

);
    created_at TIMESTAMP DEFAULT NOW()
    status VARCHAR(50) DEFAULT 'Pending',
    total_amount DECIMAL(10,2),
    user_id INT REFERENCES users(id),
    id SERIAL PRIMARY KEY,
CREATE TABLE IF NOT EXISTS orders (

);
    UNIQUE(user_id, book_id)
    created_at TIMESTAMP DEFAULT NOW(),
    comment TEXT,
    rating INT CHECK (rating BETWEEN 1 AND 5),
    book_id INT REFERENCES books(id) ON DELETE CASCADE,
    user_id INT REFERENCES users(id) ON DELETE CASCADE,
    id SERIAL PRIMARY KEY,
CREATE TABLE IF NOT EXISTS reviews (

);
    UNIQUE(user_id, book_id)
    quantity INT DEFAULT 1,
    book_id INT REFERENCES books(id) ON DELETE CASCADE,
    user_id INT REFERENCES users(id) ON DELETE CASCADE,
    id SERIAL PRIMARY KEY,
CREATE TABLE IF NOT EXISTS cart_items (

);
    created_at TIMESTAMP DEFAULT NOW()
    stock INT DEFAULT 0,
    category VARCHAR(100),
    cover_image_url TEXT,
    description TEXT,
    price DECIMAL(10,2) NOT NULL,
    author VARCHAR(255) NOT NULL,
    title VARCHAR(255) NOT NULL,
    id SERIAL PRIMARY KEY,
CREATE TABLE IF NOT EXISTS books (

);
    created_at TIMESTAMP DEFAULT NOW()
    role VARCHAR(20) DEFAULT 'User',
    password_hash TEXT NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    username VARCHAR(100) UNIQUE NOT NULL,
    id SERIAL PRIMARY KEY,
CREATE TABLE IF NOT EXISTS users (


