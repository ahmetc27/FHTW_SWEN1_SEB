using Npgsql;
using SEB.Interfaces;
using System.Data;

namespace SEB.Repositories;
public class UserRepository : BaseRepository, IUserRepository
{
    public bool Exists(string username)
    {
        // check if user exists in database
        // return true or false

        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM users WHERE username = @username LIMIT 1";
        AddParameterWithValue(command, "@username", DbType.String, username);

        using IDataReader reader = command.ExecuteReader();
        return reader.Read();
    }

    public void AddUser(string username, string password)
    {
        // add user to database
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "INSERT INTO users (username, password) VALUES (@username, @password)";
        AddParameterWithValue(command, "@username", DbType.String, username);
        AddParameterWithValue(command, "@password", DbType.String, password);
        command.ExecuteNonQuery();
    }
}
