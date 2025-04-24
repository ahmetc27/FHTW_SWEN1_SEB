using System.Data;
using Npgsql;

namespace SEB.Repositories;

public class SessionRepository : BaseRepository, ISessionRepository
{
    public bool ExistToken(string token)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM users WHERE token=@token LIMIT 1";
        AddParameterWithValue(command, "@token", DbType.String, token);

        using IDataReader reader = command.ExecuteReader();
        return reader.Read();
    }

    public void SaveToken(string username, string password, string token)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE users SET token=@token WHERE username=@username AND password=@password";
        AddParameterWithValue(command, "@username", DbType.String, username);
        AddParameterWithValue(command, "@password", DbType.String, password);
        AddParameterWithValue(command, "@token", DbType.String, token);
        
        command.ExecuteNonQuery();
    }
}