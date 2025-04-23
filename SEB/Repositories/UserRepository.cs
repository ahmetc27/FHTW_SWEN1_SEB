using Npgsql;
using SEB.Interfaces;
using SEB.Models;
using System.Data;

namespace SEB.Repositories;
public class UserRepository : BaseRepository, IUserRepository
{
    public bool ExistUsername(string username)
    {
        // for post /users duplicate username: check if username exists in database
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM users WHERE username = @username LIMIT 1";
        AddParameterWithValue(command, "@username", DbType.String, username);

        using IDataReader reader = command.ExecuteReader();
        return reader.Read();
    }

    public User? AddUser(string username, string password)
    {
        // add user (username, password) to database
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "INSERT INTO users (username, password) VALUES (@username, @password) RETURNING *";
        AddParameterWithValue(command, "@username", DbType.String, username);
        AddParameterWithValue(command, "@password", DbType.String, password);

        using IDataReader reader = command.ExecuteReader();
        if(reader.Read())
        {
            User user = new User()
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Elo = reader.GetInt32(3),
                Token = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Bio = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Image = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
            };
            return user;
        }
        return null;
    }

    public User? GetUser(string username, string password)
    {
        // for post /sessions: checks if username AND password is in db
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM users WHERE username=@username AND password=@password";
        AddParameterWithValue(command, "@username", DbType.String, username);
        AddParameterWithValue(command, "@password", DbType.String, password);

        using IDataReader reader = command.ExecuteReader();
        if(reader.Read())
        {
            User user = new User()
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Elo = reader.GetInt32(3),
                Token = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Bio = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Image = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
            };
            return user;
        }
        return null;
    }
}
