using Npgsql;

namespace Online_BookStore__System.Database;

public class DbHelper
{
    private readonly string _connectionString;

    public DbHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}

