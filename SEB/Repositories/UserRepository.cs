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

    public User AddUser(string username, string password)
    {
        // add user (username, password) to database
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "INSERT INTO users (username, password) VALUES (@username, @password) RETURNING id, username, password, elo, token, name, bio, image";
        AddParameterWithValue(command, "@username", DbType.String, username);
        AddParameterWithValue(command, "@password", DbType.String, password);

        using IDataReader reader = command.ExecuteReader();
        if(reader.Read())
        {
            return new User
            {
                UserId = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Elo = reader.GetInt32(3),
                Token = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Name = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Bio = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                Image = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
            };
        }
        throw new Exception("Unexpected error: User could not be inserted into the database.");
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
            return new User
            {
                UserId = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Elo = reader.GetInt32(3),
                Token = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Name = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Bio = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                Image = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
            };
        }
        return null;
    }

    public User? GetUserByUsernameAndToken(string username, string token)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM users WHERE username=@username AND token=@token";
        AddParameterWithValue(command, "@username", DbType.String, username);
        AddParameterWithValue(command, "@token", DbType.String, token);

        using IDataReader reader = command.ExecuteReader();
        if(reader.Read())
        {
            User user = new User()
            {
                UserId = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Elo = reader.GetInt32(3),
                Token = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Name = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Bio = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                Image = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
            };
            return user;
        }
        return null;
    }

    public void UpdateUserProfile(User user)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE users SET name=@name, bio=@bio, image=@image WHERE token=@token";
        AddParameterWithValue(command, "@token", DbType.String, user.Token);
        AddParameterWithValue(command, "@name", DbType.String, user.Name);
        AddParameterWithValue(command, "@bio", DbType.String, user.Bio);
        AddParameterWithValue(command, "@image", DbType.String, user.Image);

        command.ExecuteNonQuery();
    }

    public int GetIdByToken(string token)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT id FROM users WHERE token=@token";
        AddParameterWithValue(command, "@token", DbType.String, token);

        using IDataReader reader = command.ExecuteReader();

        if(reader.Read())
            return reader.GetInt32(0);

        return 0;
    }
}
