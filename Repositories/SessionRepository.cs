using SEB.Models;
using System.Data;
using Npgsql;

namespace SEB.Repositories
{
    public class SessionRepository(string connectionString)
    {
        public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
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
    }
}