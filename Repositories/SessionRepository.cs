using SEB.Models;
using System.Data;
using Npgsql;

namespace SEB.Repositories
{
    public class SessionRepository : BaseRepository
    {
        public bool SaveToken(User user)
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            using IDbCommand command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "UPDATE users SET token = @token WHERE id=@id";
            AddParameterWithValue(command, "token", DbType.String, user.Token);
            AddParameterWithValue(command, "id", DbType.Int32, user.Id);
            
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool ExistToken(string token)
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            using IDbCommand command = connection.CreateCommand();
            connection.Open();

            command.CommandText = @"SELECT 1 FROM users WHERE token = @token LIMIT 1;";
            AddParameterWithValue(command, "token", DbType.String, token);

            using IDataReader reader = command.ExecuteReader();
            return reader.Read();
        }

        public string GetUsernameByToken(string token)
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            using IDbCommand command = connection.CreateCommand();
            connection.Open();

            command.CommandText = @"SELECT username FROM users WHERE token=@token";
            AddParameterWithValue(command, "token", DbType.String, token);

            string username = "";

            using IDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                username = reader.GetString(0);
            }
            return username;
        }
    }
}