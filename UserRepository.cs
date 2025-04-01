using Npgsql;
using System;
using System.Data;

public class UserRepository
{
    private readonly string connectionString;

    public UserRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void Add(User user)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        using IDbCommand command = connection.CreateCommand();
        connection.Open();

        command.CommandText = "INSERT INTO users (username, password) " +
            "VALUES (@username, @password) RETURNING id";
        AddParameterWithValue(command, "username", DbType.String, user.Username ?? throw new ArgumentNullException(nameof(user.Username)));
        AddParameterWithValue(command, "password", DbType.String, user.Password ?? throw new ArgumentNullException(nameof(user.Password)));
        user.Id = (int)(command.ExecuteScalar() ?? 0);
    }
    public IEnumerable<User> GetAll()
    {
        List<User> result = [];

        using IDbConnection connection = new NpgsqlConnection(connectionString);
        using IDbCommand command = connection.CreateCommand();
        connection.Open();
        command.CommandText = @"SELECT id, username, password FROM users";

        using(IDataReader reader = command.ExecuteReader())
        while(reader.Read())
        {
            result.Add(new User() {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2)
            });
        }
        return result;
    }

    public User? GetById(int? id)
    {
        if(id == null)
            throw new ArgumentException("Id must not be null");

        using IDbConnection connection = new NpgsqlConnection(connectionString);
        using IDbCommand command = connection.CreateCommand();
        connection.Open();
        command.CommandText = @"SELECT id, username, password FROM users WHERE id=@id";
        AddParameterWithValue(command, "id", DbType.Int32, id);

        using IDataReader reader = command.ExecuteReader();
        if(reader.Read())
        {
            return new User()
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2)
            };
        }
        return null;
    }

    public void Update(User user)
    {
        if(user.Id == null)
            throw new ArgumentException("Id must not be null");

        using IDbConnection connection = new NpgsqlConnection(connectionString);
        using IDbCommand command = connection.CreateCommand();
        connection.Open();
        command.CommandText = "UPDATE users SET name=@name, password=@password WHERE id=@id";
        AddParameterWithValue(command, "id", DbType.Int32, user.Id);
        AddParameterWithValue(command, "username", DbType.String, user.Username ?? throw new ArgumentNullException(nameof(user.Username)));
        AddParameterWithValue(command, "password", DbType.String, user.Password ?? throw new ArgumentNullException(nameof(user.Password)));
        command.ExecuteNonQuery();
    }

    public void Delete(User user)
    {
        if(user.Id == null)
            throw new ArgumentException("Id must not be null");

        using IDbConnection connection = new NpgsqlConnection(connectionString);
        using IDbCommand command = connection.CreateCommand();
        connection.Open();
        command.CommandText = "DELETE FROM users WHERE id=@id";
        AddParameterWithValue(command, "id", DbType.Int32, user.Id);
        command.ExecuteNonQuery();
    }

    public void DeleteAll()
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        using IDbCommand command = connection.CreateCommand();
        connection.Open();
        command.CommandText = "DELETE FROM users";
        command.ExecuteNonQuery();
    }

    public bool ExistsByUsername(string username)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        using IDbCommand command = connection.CreateCommand();
        connection.Open();
        command.CommandText = "SELECT COUNT(*) FROM users WHERE username = @username";
        AddParameterWithValue(command, "username", DbType.String, username);

        var result = command.ExecuteScalar();
        int count = Convert.ToInt32(result);
        return count > 0;
    }


    public bool CheckCredentials(string username, string password)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";
        AddParameterWithValue(command, "username", DbType.String, username);
        AddParameterWithValue(command, "password", DbType.String, password);

        connection.Open();
        var result = command.ExecuteScalar();
        int count = Convert.ToInt32(result);
        return count == 1;
    }

    public User? GetByUsername(string username)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        using IDbCommand command = connection.CreateCommand();
        connection.Open();
        command.CommandText = @"SELECT id, username, password FROM users WHERE username=@username";
        AddParameterWithValue(command, "username", DbType.String, username);

        using IDataReader reader = command.ExecuteReader();
        if(reader.Read())
        {
            return new User()
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2)
            };
        }
        return null;
    }
    public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
    {
        var parameter = command.CreateParameter();
        parameter.DbType = type;
        parameter.ParameterName = parameterName;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}